using Lore.Game.Buildings;
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

        public UnityEvent<BuildingConstructionItem> onBuy;


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
            onBuy?.Invoke(this);
        }

        public bool CanBuy(float currentMoney)
        {
            return currentMoney >= data.Cost;
        }

        public BuildingData GetData()
        {
            return data;
        }
    }

}
