using Lore.Game.Buildings;
using Lore.Game.Characters;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.UI
{
    public class BuildingsConstructionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject buildingConstructionPrefab;
        [SerializeField] private Transform buildingDataHolder = null;

        public Action onPanelClose;

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
            onPanelClose?.Invoke(); 
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
                item.onBuySuccess.AddListener(OnItemBought);
                item.onBuyFail.AddListener(OnItemBuyFail);
                spawnedItems.Add(item);
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
            BuildingData.BuildingType bType = item.GetData().Type;
            BuildingData data = item.GetData();
            Close();
            BuildingManager.Instance.SelectBuildingForConstruction(data);
        }
        private void OnItemBuyFail(BuildingConstructionItem item, MoneyManager.PurchaseFailedReason reason)
        {
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.Error("Unable to purchase", $"You do not have enough money to buy a {item.GetData().Name}");
                return;
            }
        }

        public void ButtonClose()
        {
            Close();
            BuildingManager.Instance.ExitConstructionMode();
        }

    }
}
