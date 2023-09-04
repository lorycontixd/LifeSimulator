using Lore.Game.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class MoneyManager : BaseManager
    {
        #region Enums
        public enum PurchaseFailedReason
        {
            NO_MONEY,
            MONEYMANAGER_MISSING,
            UNKNOWN
        }
        #endregion

        #region Singleton
        private static MoneyManager _instance;
        public static MoneyManager Instance { get { return _instance; } }

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

        public float Money { get; private set; }
        [Range(0.4f,1f)] public float MinSalaryModifierWhenSad = 0.8f;

        [Header("Simulation Settings")]
        [SerializeField] private float initialMoney = 50f;
        public float monthlySalary = 500f;

        public Action<float, float> onMoneyChange;

        private float oldMoneyValue;


        public override void Start()
        {
            Money = initialMoney;
            base.Start();
        }

        #region Event Listeners
        public void OnNewDay()
        {
            //GetMoney(monthlySalary / 30f);
        }
        #endregion

        public void GetMoney(float value)
        {
            value = Mathf.Abs(value);
            oldMoneyValue = Money;
            Money += value;
            onMoneyChange?.Invoke(oldMoneyValue, Money);
        }
        public void SpendMoney(float value)
        {
            value = Mathf.Abs((float)value);
            oldMoneyValue = Money;
            Money -= value;
            onMoneyChange?.Invoke(oldMoneyValue, Money);
            if (Money <= 30f)
            {
                GamePlayer p = FindFirstObjectByType<GamePlayer>();
                if (!p.beliefs.HasState("NeedsMoney"))
                {
                    p.beliefs.AddState("NeedsMoney", true);
                    p.AddGoal("StealMoney", 5, true);
                }   
            }
        }

        public bool CanAfford(float cost)
        {
            return cost <= Money;
        }

        public void GetDailySalary()
        {
            float dailySalary = monthlySalary / TimeManager.Instance.DaysInMonth;
            GamePlayer p = FindFirstObjectByType< GamePlayer>();
            bool isSad = false;
            if (p != null)
            {
                isSad = p.beliefs.HasState("IsSad");
            }
            float sadnessMod = isSad ? MinSalaryModifierWhenSad : 1f;
            GetMoney(dailySalary * sadnessMod);
        }
    }

}
