using Lore.Game.Buildings;
using Lore.Game.Managers;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Lore.Game.Buildings
{
    public class Building : MonoBehaviour
    {
        public enum BuildingState
        {
            NONE,
            SELECTION,
            CONSTRUCTION,
            BUILT
        }


        const int nRays = 5;
        public BuildingData data;
        public BuildingState state = BuildingState.NONE;
        [SerializeField] private GameObject surface = null;

        [Header("UI")]
        [SerializeField] private Canvas buildingLabelCanvas = null;
        [SerializeField] private ProgressBar constructionProgressBar;

        public Action<Building> onBuildingConstructionStarted;
        public Action<Building> onBuildingConstructionFinished;

        private float ConstructionTimestamp;


        private void Start()
        {
            if (data != null)
            {
                UpdateUI();
            }
            if (surface == null)
            {
                surface = transform.Find("Surface").gameObject;
            }
            if (buildingLabelCanvas == null)
            {
                buildingLabelCanvas = transform.Find("BuildingCanvas").GetComponent<Canvas>();
            }
            if (constructionProgressBar == null)
            {
                constructionProgressBar = buildingLabelCanvas.transform.Find("ConstructionProgressBar").GetComponent <ProgressBar>();
            }
            constructionProgressBar.gameObject.SetActive(false);

            if (state == BuildingState.BUILT && BuildingManager.Instance != null)
            {
                BuildingManager.Instance.AddConstructedBuilding(this);
            }
            
            
        }
        private void Update()
        {
            if (state == BuildingState.CONSTRUCTION)
            {
                ConstructionTimestamp += Time.deltaTime;
                constructionProgressBar.ChangeValue((float)(ConstructionTimestamp / data.ConstructionDuration) * 100f);
                constructionProgressBar.UpdateUI();
                if (ConstructionTimestamp >= data.ConstructionDuration)
                {
                    EndConstruction();
                }
            }
        }

        public void AssignBuilding(BuildingData data)
        {
            this.data = data;
            NavMeshObstacle obs = GetComponent<NavMeshObstacle>();
            obs.enabled = data.Type != BuildingData.BuildingType.NONE;
            UpdateUI();
        }
        private void UpdateUI()
        {
            if (buildingLabelCanvas != null && data.Type != BuildingData.BuildingType.NONE)
            {
                buildingLabelCanvas.GetComponentInChildren<TextMeshProUGUI>().text = data.Type.ToString().ToLower().Capitalize();
            }
        }



        #region Construction
        public void SelectForConstruction()
        {
            surface.SetActive(false);
            state = BuildingState.SELECTION;
        }
        public void StartConstruction()
        {
            state = BuildingState.CONSTRUCTION;
            ConstructionTimestamp = 0f;
            surface.SetActive(true);
            constructionProgressBar.gameObject.SetActive(true);
            constructionProgressBar.ChangeValue(0f);
            constructionProgressBar.UpdateUI();
            onBuildingConstructionStarted?.Invoke(this);
        }
        public void EndConstruction()
        {
            state = BuildingState.BUILT;
            surface.SetActive(true);
            constructionProgressBar.gameObject.SetActive(false);
            onBuildingConstructionFinished?.Invoke(this);
        }
        private IEnumerator StartConstructionCo()
        {
            yield return null;
        }

        public bool CheckSurface()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                Bounds bounds = collider.bounds;
                Vector3 loweredY = bounds.center - Vector3.up * bounds.size.y / 2;
                Vector3[] points = new Vector3[nRays] {
                    bounds.center,
                    loweredY + Vector3.right * bounds.size.x / 2,
                    loweredY - Vector3.right * bounds.size.x / 2,
                    loweredY + Vector3.forward * bounds.size.z / 2,
                    loweredY - Vector3.forward * bounds.size.z / 2
                };

                bool isBuildable = true;
                for (int i=0; i<nRays; i++)
                {
                    Ray ray = new Ray(points[i], Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        Surface surface = hit.transform.GetComponent<Surface>();
                        if (surface != null)
                        {
                            if (!surface.IsBuildable)
                            {
                                isBuildable = false;
                                break;
                            }
                        }
                        else
                        {
                            isBuildable = false;
                            break;
                        }
                    }
                }
                return isBuildable;
            }
            else
            {
                return false;
            }
        }
        
        #endregion

    }
}
