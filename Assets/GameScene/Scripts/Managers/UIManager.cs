using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class UIManager : BaseManager
    {
        #region Singleton
        private static UIManager _instance;
        public static UIManager Instance { get { return _instance; } }

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

        [Header("Panels")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private BuildingCommandsPanel buildingCommandsPanel;
        [SerializeField] private PlayerStatsPanel playerStatsPanel;


        public override void Start()
        {
            mainCanvas.gameObject.SetActive(true);
            buildingCommandsPanel.gameObject.SetActive(true);
            if (playerStatsPanel != null)
            {
                playerStatsPanel.gameObject.SetActive(true);
                if (MoneyManager.Instance != null)
                {
                    playerStatsPanel.SetMoney(MoneyManager.Instance.Money);
                    MoneyManager.Instance.onMoneyChange += OnMoneyChange;
                }
                else
                {
                }
            }
            
            base.Start();
        }

        private void OnMoneyChange(float oldValue, float newValue)
        {
            playerStatsPanel.SetMoney(newValue);
        }
    }
}
