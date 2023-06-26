using Lore.Game.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class SicknessManager : BaseManager
    {
        #region Singleton
        private static SicknessManager _instance;
        public static SicknessManager Instance { get { return _instance; } }

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

        [Header("Settings")]
        [SerializeField] private bool UseRandomDisease = true;

        // Events
        public Action onNewDisease;

        private GamePlayer player;
        private NewPlayerStats playerStats;
        private float fatigueTimestamp;
        private bool isInTiredState = false;


        public override void Start()
        {
            player = FindFirstObjectByType<GamePlayer>();
            playerStats = player.GetComponent<NewPlayerStats>();

            playerStats.onStatCritical += OnStatCritical;
            base.Start();
        }
        private void Update()
        {
            if (isInTiredState)
            {
                fatigueTimestamp += Time.deltaTime;
            }   
        }


        private void CalculateRandomDisease()
        {
            // if no disease was given, increase chances

        }
        private void OnStatCritical(string stat, float value)
        {
            if (stat == "Fatigue")
            {
                isInTiredState = true;
            }
        }

        private void OnFatigueReset()
        {
            isInTiredState = false;
            fatigueTimestamp = 0f;
        }

    }

}
