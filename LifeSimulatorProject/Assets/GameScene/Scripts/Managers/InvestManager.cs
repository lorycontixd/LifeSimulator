using Lore.Game.Characters;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class InvestManager : BaseManager
    {
        #region Enums
        #endregion

        #region Singleton
        private static InvestManager _instance;
        public static InvestManager Instance { get { return _instance; } }

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

        [SerializeField] private List<Investment> allInvestments = new List<Investment>();
        [SerializeField] private bool autoEnumerateInvestments = true;

        public int InvestmentsCount { get { return currentInvestments.Count; } }
        public float InvestmentsMoneySpent { get; private set; }
        public float InvestmentsMoneyGained { get; private set; }
        public List<Investment> PendingInvestments { get { return pendingInvestments; } }
        public bool HasPendingInvestment { get { if (pendingInvestments == null) return false; return pendingInvestments.Count > 0; } }

        private List<Investment> currentInvestments;
        private List<Investment> pendingInvestments = null;
        private GamePlayer player;


        public override void Start()
        {
            currentInvestments = new List<Investment>();
            pendingInvestments = new List<Investment>();
            player = FindFirstObjectByType<GamePlayer>();

            TimeManager.Instance.onNewDay += OnNewDay;

            base.Start();
        }

        private void OnNewDay(int dayNumber)
        {
            bool isBonusDay = dayNumber % TimeManager.Instance.DaysInMonth == 0;
            GiveReturns(isBonusDay ? 2f : 1f);
        }

        private void GiveReturns(float bonus = 1f)
        {
            foreach (Investment investment in currentInvestments)
            {
                float gainedValue = bonus * investment.MonthlyPassiveIncome / TimeManager.Instance.DaysInMonth;
                InvestmentsMoneyGained += gainedValue;
                MoneyManager.Instance.GetMoney(gainedValue);
            }
        }


        public void InvestmentSelected(Investment investment)
        {
            if (BuildingManager.Instance != null)
            {
                if (BuildingManager.Instance.IsBuildingConstructed(Buildings.BuildingData.BuildingType.BANK))
                {
                    if (!player.beliefs.HasState("PendingInvestment"))
                    {
                        player.beliefs.AddState("PendingInvestment", true);
                        player.AddGoal("GetInvestment", 3, true);
                    }
                    MoneyManager.Instance.SpendMoney(investment.Cost);
                    NotificationManager.Instance.Info("Investment submitted", "You have submitted the investment request. Wait for the player to visit the bank");
                    pendingInvestments.Add(investment);
                }
                else
                {
                    NotificationManager.Instance.Error("Building missing", "You cannot start an investment without having constructed a bank");
                }
            }
            else
            {
                NotificationManager.Instance.Error("Unknown error", "Investment could not be started");
            }
            ExitInvestmentMode();
        }

        public void AcceptPendingInvestments()
        {
            player.beliefs.RemoveState("PendingInvestment");
            foreach(var investment in pendingInvestments)
            {
                AddInvestment(investment);
                InvestmentsMoneySpent += investment.Cost;
            }
            pendingInvestments.Clear();
        }

        public void AddInvestment(Investment investment)
        {
            if (autoEnumerateInvestments)
            {
                investment.Setup(currentInvestments.Count);
            }
            currentInvestments.Add(investment);
        }
        public bool RemoveInvestment(Investment investment)
        {
            if (currentInvestments.Contains(investment))
            {
                currentInvestments.Remove(investment);
                return true;
            }
            return false;
        }
        public bool RemoveInvestment(int id)
        {
            Investment investment = GetInvestment(id);
            if (investment != null)
            {
                currentInvestments.Remove(investment);
                return true;
            }
            return false;
        }
        public List<Investment> GetBoughtInvestments()
        {
            return currentInvestments;
        }
        public List<Investment> GetPossibleInvestments()
        {
            return allInvestments;
        }
        public Investment GetInvestment(int id)
        {
            return currentInvestments.FirstOrDefault(i => i.ID == id);
        }
        public bool HasInvestment(string name)
        {
            return currentInvestments.Any(i => i.Name == name);
        }

        public void EnterInvestmentMode()
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
        public void ExitInvestmentMode()
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
    }

}
