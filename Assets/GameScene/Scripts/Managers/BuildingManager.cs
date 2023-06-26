using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lore.Game.Buildings;
using MyBuilding = Lore.Game.Buildings.Building;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Cinemachine;

namespace Lore.Game.Managers
{
    public class BuildingManager : BaseManager
    {
        #region BuildingSetup
        [Serializable]
        public struct BuildingSetup
        {
            public BuildingData.BuildingType type;
            public int maxNumber;
        }
        #endregion

        #region Singleton
        private static BuildingManager _instance;
        public static BuildingManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        [SerializeField] private List<BuildingSetup> setupData = new List<BuildingSetup>();
        public List<BuildingData> buildingDatas = new List<BuildingData>();
        [SerializeField] private List<GameObject> buildingPrefabs = new List<GameObject>();
        [SerializeField] private Transform buildingHolder = null;
        [SerializeField] private List<Transform> parkSpots = new List<Transform>();
        public LayerMask groundLayer;
        public int nearbyBuildingRange = 5;
        private List<MyBuilding> constructedBuildings = new List<MyBuilding>();
        public MyBuilding WorkBuilding { get; private set; }
        public int HousesCount { get { return constructedBuildings.Where(b => b.data.Type == BuildingData.BuildingType.HOUSE).Count(); } }

        // Construction
        [Space(10f)]
        [Header("Construction")]
        [SerializeField] private NavmeshBaker navmeshBaker;
        [SerializeField] private Material ValidPlacementMaterial;
        [SerializeField] private Material InvalidPlacementMaterial;
        private MyBuilding temporaryConstructionBuilding = null;
        private Rigidbody temporaryConstructionBuildingRb = null;
        private BoxCollider temporaryConstructionBuildingBox = null;
        private bool temporaryConstructionBuildingIsValid = false;
        private Material temporaryConstructionBuildingMat = null;
        private Surface temporaryConstructionSurface = null;

        [Space(5f)]
        public UnityEvent<MyBuilding> onBuildingConstructed;
        private System.Random random = new System.Random();


        public override void Start()
        {
            random = new System.Random();
            bool setupValid = ValidateSetupData();
            constructedBuildings = FindObjectsByType<MyBuilding>(FindObjectsSortMode.None).ToList();
            // Pick work company
            var companies = constructedBuildings.Where(b => b.GetComponent<MyBuilding>().data.Type == BuildingData.BuildingType.COMPANY).ToList();
            if (companies.Count <= 0)
            {
                Debug.LogError($"Could not fetch work building because no initial Company Building was set. Please spawn a company building at editor time.");
                return;
            }
            int index = random.Next(companies.Count);
            WorkBuilding = companies[index];
            base.Start();
        }
        private void Update()
        {
            if (temporaryConstructionBuilding != null)
            {
                temporaryConstructionBuildingIsValid = ValidatePlacement();
                if (temporaryConstructionBuildingMat == null)
                {
                    temporaryConstructionBuildingMat = temporaryConstructionBuilding.GetComponent<MeshRenderer>().material;
                }
                if (temporaryConstructionBuildingIsValid)
                {
                    temporaryConstructionBuilding.GetComponent<MeshRenderer>().material = ValidPlacementMaterial;


                }
                else
                {
                    temporaryConstructionBuilding.GetComponent<MeshRenderer>().material = InvalidPlacementMaterial;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (temporaryConstructionBuildingIsValid)
                    {
                        Place();
                    }
                }
            }
        }
        private bool ValidateSetupData()
        {
            foreach(var d in buildingDatas)
            {
                List<BuildingSetup> buildingSetupsOfType = setupData.Where(s => s.type == d.Type).ToList();
                if (buildingSetupsOfType.Count != 1)
                {
                    Debug.LogWarning($"[BuildingManager->ValidateSetup] Building setup with type {d.Type} must occur only once. Ignoring type");
                    return false;
                }
            }
            return true;
        }


        #region Getters
        // Generic getters
        public bool IsBuildingConstructed(BuildingData.BuildingType buildingType)
        {
            return constructedBuildings.Any(b => b.data.Type == buildingType && b.state == MyBuilding.BuildingState.BUILT);
        }

        public MyBuilding GetFirstBuildingByType(BuildingData.BuildingType buildingType)
        {
            return constructedBuildings.FirstOrDefault(b => b.data.Type == buildingType && b.state == MyBuilding.BuildingState.BUILT) ?? null;
        }

        public List<MyBuilding> GetAllBuildingsByType(BuildingData.BuildingType type)
        {
            return constructedBuildings.Where(b => b.data.Type == type && b.state == MyBuilding.BuildingState.BUILT).ToList();
        }
        public MyBuilding GetRandomBuildingByType(BuildingData.BuildingType type)
        {
            List<MyBuilding> buildingsOfType = GetAllBuildingsByType(type);
            return buildingsOfType[Random.Range(0, buildingsOfType.Count)];
        }

        public MyBuilding GetClosestBuildingByType(BuildingData.BuildingType type, Vector3 position)
        {
            MyBuilding closest = null;
            foreach (MyBuilding building  in constructedBuildings)
            {
                if (building.data.Type == type && building.state == MyBuilding.BuildingState.BUILT)
                {
                    if (closest == null)
                    {
                        closest = building;
                    }
                    else
                    {
                        if (Vector3.Distance(closest.transform.position, position) > Vector3.Distance(position, building.transform.position))
                        {
                            closest = building;
                        }
                    }
                }
            }
            return closest;
        }

        public MyBuilding GetRandomBuilding()
        {
            var builtBuildings = constructedBuildings.Where(b => b.state == MyBuilding.BuildingState.BUILT).ToList();
            return builtBuildings[Random.Range(0, builtBuildings.Count)];
        }

        // Specific getters
        public MyBuilding GetFirstAvailableHouse()
        {
            // check maximum capacity for example
            return GetFirstBuildingByType(BuildingData.BuildingType.HOUSE);
        }
        public Transform GetRandomPark()
        {
            return parkSpots[Random.Range(0, parkSpots.Count)];
        }
        #endregion



        #region Construction
        public void EnterConstructionMode()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.Pause();
            }
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SelectFixedCamera();
            }
        }
        public void ExitConstructionMode()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.Resume();
            }
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SelectMainCamera();
            }
        }

        public void AddConstructedBuilding(MyBuilding building)
        {
            constructedBuildings.Add(building);
        }

        public void SelectBuildingForConstruction(BuildingData data, GameObject buildingGo = null)
        {
            if (buildingGo == null)
            {
                int index = Random.Range(0, buildingPrefabs.Count);
                buildingGo = buildingPrefabs[index];
            }
            GameObject clone = Instantiate(buildingGo, null);
            temporaryConstructionBuilding = clone.GetComponent<MyBuilding>();
            temporaryConstructionBuilding.AssignBuilding(data);
            temporaryConstructionBuildingBox = clone.GetComponent<BoxCollider>();
            temporaryConstructionBuildingRb = clone.GetComponent<Rigidbody>();
            temporaryConstructionBuilding.SelectForConstruction();
        }
        public void Place()
        {
            constructedBuildings.Add(temporaryConstructionBuilding);
            float getGroundHeight = GetGroundDistance(temporaryConstructionBuilding.transform.position - Vector3.up * temporaryConstructionBuildingBox.bounds.size.y/2);
            //temporaryConstructionBuilding.transform.Translate(Vector3.down * 1f * getGroundHeight, Space.World);
            temporaryConstructionBuildingBox = null;
            temporaryConstructionBuildingRb = null;
            temporaryConstructionBuildingIsValid = false;
            if (temporaryConstructionBuildingMat != null)
            {
                temporaryConstructionBuilding.GetComponent<MeshRenderer>().material = temporaryConstructionBuildingMat;
                temporaryConstructionBuildingMat = null;
            }
            Transform parent = null;
            if (temporaryConstructionSurface != null)
            {
                Transform buildingSection = temporaryConstructionSurface.transform.Find("Buildings");
                if (buildingSection != null)
                {
                    parent = buildingSection;
                }
                else
                {
                    parent = temporaryConstructionSurface.transform;
                }
            }
            temporaryConstructionBuilding.transform.parent = parent;
            temporaryConstructionBuilding.onBuildingConstructionFinished += OnBuildingConstructionFinished;
            temporaryConstructionBuilding.StartConstruction();
            ExitConstructionMode();
            temporaryConstructionBuilding = null;


            if (navmeshBaker != null)
            {
                navmeshBaker.Bake();
            }
        }

        private void OnBuildingConstructionFinished(MyBuilding building)
        {
            if (navmeshBaker != null)
            {
                navmeshBaker.Bake();
            }
            BuildingManager.Instance.AddConstructedBuilding(building);
        }

        public void CancelConstruction()
        {
            Destroy(temporaryConstructionBuildingRb);
            temporaryConstructionBuilding = null;
            temporaryConstructionBuildingBox = null;
            temporaryConstructionBuildingRb = null;
            if (TimeManager.Instance != null)
            {
                if (!TimeManager.Instance.IsActive)
                {
                    TimeManager.Instance.Resume();
                }
            }
        }

        public bool ValidatePlacement()
        {

            temporaryConstructionBuildingRb.useGravity = false;
            temporaryConstructionBuildingRb.isKinematic = true;
            Ray ray = CameraManager.Instance.activeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 point;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                point = hit.point;
                //point.y += temporaryConstructionBuildingBox.bounds.size.y;
                Vector3 newPoint = point + Vector3.down * temporaryConstructionBuildingBox.bounds.size.y;
                temporaryConstructionBuildingRb.position = point;
                Surface surface = hit.transform.GetComponent<Surface>();
                if (surface != null)
                {
                    if (surface.IsBuildable)
                    {
                        temporaryConstructionSurface = surface;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool AreBuildingsNearby()
        {
            Collider[] colliders = Physics.OverlapBox(temporaryConstructionBuilding.transform.position, new Vector3(nearbyBuildingRange, nearbyBuildingRange, nearbyBuildingRange));

            for (int i = 0; i < colliders.Length; i++)
            {
                Building building = colliders[i].GetComponent<Building>();
                if (building != null)
                {
                    return true;
                }
            }
            return false;
        }
        private float GetGroundDistance(Vector3 origin)
        {
            if (BuildingManager.Instance == null)
            {
                return -1f;
            }
            RaycastHit hit;
            if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, BuildingManager.Instance.groundLayer))
            {
                return hit.distance;
            }
            else
            {
                return -1f;
            }
        }
        #endregion
    }

}
