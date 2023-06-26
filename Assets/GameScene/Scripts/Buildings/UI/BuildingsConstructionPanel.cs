using Lore.Game.Buildings;
using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.UI
{
    public class BuildingsConstructionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject buildingConstructionPrefab;
        [SerializeField] private Transform buildingDataHolder = null;
        private List<BuildingConstructionItem> spawnedItems = new List<BuildingConstructionItem>();


        private void Start()
        {
        }


        public void Open()
        {
            gameObject.SetActive(true);
            buildingDataHolder.gameObject.SetActive(true);
            SpawnItems();
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void SpawnItems() {
            if (BuildingManager.Instance == null)
            {
                return;
            }
            ClearItems();
            foreach(BuildingData data in BuildingManager.Instance.buildingDatas)
            {
                GameObject clone = Instantiate(buildingConstructionPrefab, buildingDataHolder);
                BuildingConstructionItem item = clone.GetComponent<BuildingConstructionItem>();
                item.SetBuilding(data);
                item.onBuy.AddListener(OnItemBought);
            }
        }
        private void ClearItems()
        {
            spawnedItems.Clear();
            for(int i=0; i<buildingDataHolder.childCount; i++)
            {
                Destroy(buildingDataHolder.GetChild(i).gameObject);
            }
        }

        public void OnItemBought(BuildingConstructionItem item)
        {
            
            BuildingData.BuildingType bType = (BuildingData.BuildingType)item.GetData().Type;
            BuildingData data = (BuildingData)item.GetData();
            Close();
            BuildingManager.Instance.SelectBuildingForConstruction(data);
            Debug.Log($"Bought {item.GetData().Name}");
        }

        public void ButtonClose()
        {
            Close();
            BuildingManager.Instance.ExitConstructionMode();
        }
    }
}
