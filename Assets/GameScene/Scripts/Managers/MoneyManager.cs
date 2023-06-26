using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class MoneyManager : BaseManager
    {
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

        [Header("Siomulation Settings")]
        public float MonthlySalary = 500f;

        public Action<float, float> onMoneyChange;

        private float oldMoneyValue;


        public override void Start()
        {
            base.Start();
        }

        #region Event Listeners
        public void OnNewDay()
        {
            GetMoney(MonthlySalary / 30f);
        }
        #endregion

        public void GetMoney(float value)
        {
            value = Mathf.Abs(value);
            oldMoneyValue = Money;
            Money -= value;
            onMoneyChange?.Invoke(oldMoneyValue, Money);
        }
        public void SpendMoney(float value)
        {
            value = Mathf.Abs((float)value);
            oldMoneyValue = Money;
            Money -= value;
            onMoneyChange?.Invoke(oldMoneyValue, Money);
        }

        public bool CanAfford(float cost)
        {
            return cost <= Money;
        }

        public void GetDailySalary()
        {
            float dailySalary = MonthlySalary / 30f;
            GetMoney(dailySalary);
        }
    }

}
