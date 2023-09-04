using Lore.Game.Buildings;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lore.Game.UI
{
    public class BuildingConstructionItem : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;

        private BuildingData data;
        private Button button = null;

        public UnityEvent<BuildingConstructionItem> onBuySuccess;
        public UnityEvent<BuildingConstructionItem, MoneyManager.PurchaseFailedReason> onBuyFail;


        private void Start()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }
        public void SetBuilding(BuildingData data)
        {
            this.data = data;
            UpdateUI();
        }
        public void UpdateUI()
        {
            if (data != null)
            {
                iconImage.sprite = data.Icon;
                nameText.text = data.Name;
                priceText.text = $"{data.Cost}€";
            }
        }
        public void Buy()
        {
            if (MoneyManager.Instance == null) { onBuyFail?.Invoke(this, MoneyManager.PurchaseFailedReason.MONEYMANAGER_MISSING); }
            if (MoneyManager.Instance.CanAfford(data.Cost))
            {
                onBuySuccess?.Invoke(this);
            }
            else
            {
                onBuyFail?.Invoke(this, MoneyManager.PurchaseFailedReason.NO_MONEY);
            }
        }

        public BuildingData GetData()
        {
            return data;
        }
    }

}
