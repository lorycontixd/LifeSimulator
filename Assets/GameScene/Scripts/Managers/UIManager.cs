using Lore.Game.Characters;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private StatesCanvasManager _statesCanvasManager;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CommandsPanel buildingCommandsPanel;
        [SerializeField] private PlayerStatsPanel playerStatsPanel;
        [SerializeField] private GameObject deathPanel;
        [SerializeField] private TextMeshProUGUI reasonText;
        [SerializeField] private TextMeshProUGUI deathInfoText;

        private NewPlayerStats playerStats;


        public new IEnumerator Start()
        {
            playerStats = FindFirstObjectByType<GamePlayer>().GetComponentInParent<NewPlayerStats>();
            playerStats.onHealthChanged += OnHealthChanged;
            playerStats.onDeath += OnDeath;
            deathPanel.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
            buildingCommandsPanel.gameObject.SetActive(true);
            yield return new WaitUntil(() => MoneyManager.Instance.IsSetup);
            MoneyManager.Instance.onMoneyChange += OnMoneyChange;
            if (playerStatsPanel != null)
            {
                playerStatsPanel.gameObject.SetActive(true);
                if (MoneyManager.Instance != null)
                {
                    playerStatsPanel.SetMoney(MoneyManager.Instance.Money);
                }
                else
                {
                }
            }
            base.Start();
        }

        private void OnDeath(NewPlayerStats.DamageReason reason)
        {
            if (buildingCommandsPanel != null)
                buildingCommandsPanel.CloseCommandsPanel();
            if (_statesCanvasManager != null)
                _statesCanvasManager.CloseAllPanels();
            deathPanel.SetActive(true);
            SetFinalInfo(reason);
            CameraManager.Instance.SelectFixedCamera();
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            playerStatsPanel.SetHealth(newValue);
        }

        private void OnMoneyChange(float oldValue, float newValue)
        {
            playerStatsPanel.SetMoney(newValue);
        }


        #region Death Panel
        public void ButtonReturnMenu()
        {

        }
        public void ButtonQuit()
        {
            Application.Quit();
        }

        public void SetFinalInfo(NewPlayerStats.DamageReason reason)
        {
            TimeSpan t = TimeSpan.FromSeconds(TimeManager.Instance.TimeSinceStart);

            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
            if (reasonText != null)
                reasonText.text = $"Reason: {reason}";
            if (deathInfoText != null)
            {
                string text = $"==> Days survived: {TimeManager.Instance.DaysPassed}\n" +
                    $"==> Total duration: {answer}\n" +
                    $"==> Final money: {MoneyManager.Instance.Money}\n" +
                    $"==> Diseases cured: {SicknessManager.Instance.TotalDiseasesCured}\n" +
                    $"==> Buildings constructed: {BuildingManager.Instance.BuildingsBuilt}\n" +
                    $"==> Population achieved: {"TBD"}\n" +
                    $"==> Money gained from investments: {InvestManager.Instance.InvestmentsMoneyGained}";
                deathInfoText.text = text;
            }
        }
        #endregion


    }
}
