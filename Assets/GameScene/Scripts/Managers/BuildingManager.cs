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
using Lore.Game.Characters;

namespace Lore.Game.Managers
{
    public class BuildingManager : BaseManager
    {
        #region Enums
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

        public int CityLevel { get; private set; } = 1;
        [SerializeField] private List<BuildingSetup> setupData = new List<BuildingSetup>();
        public List<BuildingData> buildingDatas = new List<BuildingData>();
        [SerializeField] private List<GameObject> buildingPrefabs = new List<GameObject>();
        [SerializeField] private Transform buildingHolder = null;
        [SerializeField] private List<Transform> parkSpots = new List<Transform>();
        public LayerMask groundLayer;
        public int nearbyBuildingRange = 5;
        [Range(50f, 150f)] public float BuildingInactivityWarningTime = 80f;
        [Range(50f, 200f)] public float BuildingInactivityLoseTime = 160f;
        public float BuildingInactivityTimeRequired { get => BuildingInactivityLoseTime - BuildingInactivityWarningTime; }
        private List<MyBuilding> constructedBuildings = new List<MyBuilding>();
        public MyBuilding WorkBuilding { get; private set; }
        public int HousesCount { get { return constructedBuildings.Where(b => b.data.Type == BuildingData.BuildingType.HOUSE).Count(); } }
        public int BuildingsBuilt { get; private set; } = 0;

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
        private float LastBuildingBuiltGameTime = 0f;
        private bool BuildingInactivityWarningSent = false;

        [Space(5f)]
        public UnityEvent<MyBuilding> onBuildingPlaced;
        public UnityEvent<MyBuilding> onBuildingConstructed;
        public Action onBuildingInactivityWarning;
        public UnityEvent<int> onLevelUpgrade;
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
            if (TimeManager.Instance.TimeSinceStart - LastBuildingBuiltGameTime > BuildingInactivityWarningTime && !BuildingInactivityWarningSent)
            {
                Managers.NotificationManager.Instance.Warning("Buildings needed", $"Town major wants more building. You have {BuildingInactivityTimeRequired} seconds to construct a building or you lose.");
                BuildingInactivityWarningSent = true;
                onBuildingInactivityWarning?.Invoke(); 
            }
            if (BuildingInactivityWarningSent && TimeManager.Instance.TimeSinceStart - LastBuildingBuiltGameTime > BuildingInactivityLoseTime)
            {
                GamePlayer player = FindFirstObjectByType<GamePlayer>();
                player.GetComponent<NewPlayerStats>().TakeDamage(200f, NewPlayerStats.DamageReason.KILLED_BY_AUTHORITIES);
                return;
            }
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
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    CancelConstruction();
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
            MyBuilding selectedBuilding = builtBuildings[Random.Range(0, builtBuildings.Count)];
            return selectedBuilding;
        }

        public MyBuilding GetRandomBuildingExcept(MyBuilding[] exceptBuilding)
        {
            var builtBuildings = constructedBuildings.Where(b => b.state == MyBuilding.BuildingState.BUILT).Except(exceptBuilding).ToList();
            MyBuilding selectedBuilding = builtBuildings[Random.Range(0, builtBuildings.Count)];
            return selectedBuilding;
        }

        // Specific getters
        public MyBuilding GetFirstAvailableHouse()
        {
            // check maximum capacity for example
            return GetFirstBuildingByType(BuildingData.BuildingType.HOUSE);
        }
        public Transform GetRandomPark(string method = "r")
        {
            if (method == "r")
            {
                return parkSpots[Random.Range(0, parkSpots.Count)];
            }else if (method == "d")
            {
                GamePlayer player = GameObject.FindFirstObjectByType<GamePlayer>();
                List<float> distancesToParks = new List<float>();
                foreach(Transform p in parkSpots)
                {
                    distancesToParks.Add(Vector3.Distance(player.transform.position, p.position));
                }

                List<float> probabilities = new List<float>();
                foreach(float p in distancesToParks)
                {
                    probabilities.Add(p / distancesToParks.Sum());
                }

                Dictionary<Transform, float> parkProbabilities = new Dictionary<Transform, float>();
                for (int i=0; i<probabilities.Count; i++)
                {
                    parkProbabilities.Add(parkSpots[i], probabilities[i]);
                }
                parkProbabilities = parkProbabilities.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                foreach(KeyValuePair<Transform, float> pair in parkProbabilities)
                {
                    if (Random.Range(0f,1f) < pair.Value)
                    {
                        return pair.Key;
                    }
                }
                return parkProbabilities.Keys.ToArray()[0];
            }
            else
            {
                return parkSpots[Random.Range(0, parkSpots.Count)];
            }
        }
        #endregion



        #region Construction
        public bool EnterConstructionMode()
        {
            GAgent player = FindFirstObjectByType<GamePlayer>();
            bool canEnter = !player.invoked;
            if (!canEnter)
            {
                return false;
            }
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.Pause();
            }
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SelectFixedCamera();
            }
            return true;
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
            onBuildingConstructed?.Invoke(building);
        }

        public void SelectBuildingForConstruction(BuildingData data)
        {
            GameObject buildingGo = null;
            if (data.buildingPrefab == null )
            {
                int index = Random.Range(0, buildingPrefabs.Count);
                buildingGo = buildingPrefabs[index];
            }
            else
            {
                if (data.buildingPrefab.GetComponent<MyBuilding>() != null)
                {
                    buildingGo = data.buildingPrefab;
                }
                else
                {
                    int index = Random.Range(0, buildingPrefabs.Count);
                    buildingGo = buildingPrefabs[index];
                }
            }
            GameObject clone = Instantiate(buildingGo, null);
            temporaryConstructionBuilding = clone.GetComponent<MyBuilding>();
            temporaryConstructionBuilding.AssignBuilding(data);
            temporaryConstructionBuildingBox = clone.GetComponent<BoxCollider>();
            temporaryConstructionBuildingRb = clone.GetComponent<Rigidbody>();
            temporaryConstructionBuilding.SelectForConstruction();
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.Info("Press <escape> to cancel", "Press <escape> to cancel building construction!");
            }
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
            MoneyManager.Instance.SpendMoney(temporaryConstructionBuilding.data.Cost);
            temporaryConstructionBuilding.StartConstruction();
            ExitConstructionMode();
            onBuildingPlaced?.Invoke(temporaryConstructionBuilding);
            LastBuildingBuiltGameTime = TimeManager.Instance.TimeSinceStart;
            temporaryConstructionBuilding = null;
            BuildingInactivityWarningSent = false;
            BuildingsBuilt++;
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
            AddConstructedBuilding(building);
            int levelVal = (int) (Mathf.Pow(BuildingsBuilt, 4/7f));
            Debug.Log($"BuildingConstructed ==> Level value: {levelVal},  actual lvl: {CityLevel}");
            if (CityLevel < levelVal)
            {
                CityLevel++;
                onLevelUpgrade.Invoke(levelVal);
            }
        }

        public void CancelConstruction()
        {
            Destroy(temporaryConstructionBuildingRb.gameObject);
            temporaryConstructionBuilding = null;
            temporaryConstructionBuildingRb = null;
            temporaryConstructionBuildingBox = null;
            temporaryConstructionBuildingIsValid = false;
            temporaryConstructionBuildingMat = null;
            temporaryConstructionSurface = null;
            ExitConstructionMode();
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
