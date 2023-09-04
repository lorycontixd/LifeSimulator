using Lore.Game.Buildings;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.UI
{
    public class InvestmentsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject investmentPrefab;
        [SerializeField] private Transform investmentDataHolder;

        public Action onPanelClose;

        private List<InvestmentPanelItem> spawnedItems = new List<InvestmentPanelItem>();


        private void Start()
        {
        }

        private void OnNewDay(int dayNumber)
        {
        }

        public void Open()
        {
            gameObject.SetActive(true);
            investmentDataHolder.gameObject.SetActive(true);
            SpawnItems();
        }

        public void Close()
        {

            gameObject.SetActive(false);
            onPanelClose?.Invoke();
        }

        

        public void SpawnItems()
        {
            if (InvestManager.Instance == null)
            {
                return;
            }
            ClearItems();
            foreach(Investment investment in InvestManager.Instance.GetPossibleInvestments())
            {
                GameObject clone = Instantiate(investmentPrefab, investmentDataHolder);
                InvestmentPanelItem item = clone.GetComponentInParent<InvestmentPanelItem>();
                item.SetInvestment(investment);
                item.onBuySuccess.AddListener(OnInvestmentSelected);
                item.onBuyFail.AddListener(OnInvestmentFail);
                spawnedItems.Add(item);
            }
        }
        private void ClearItems()
        {
            spawnedItems.Clear();
            for (int i = 0; i < investmentDataHolder.childCount; i++)
            {
                Destroy(investmentDataHolder.GetChild(i).gameObject);
            }
        }
        private void OnInvestmentSelected(InvestmentPanelItem item)
        {
            Investment i = item.GetData();
            Close();
            if (InvestManager.Instance != null)
            {
                InvestManager.Instance.InvestmentSelected(i);
            }
        }
        private void OnInvestmentFail(InvestmentPanelItem item, MoneyManager.PurchaseFailedReason reason)
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
            InvestManager.Instance.ExitInvestmentMode();
        }
    }

}
