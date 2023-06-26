using GOAP;
using Lore.Game.Managers;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationManager = Lore.Game.Managers.NotificationManager;

namespace Lore.Game.Characters
{
    public class GamePlayer : GAgent
    {
        NewPlayerStats playerStats;
        private Citizen talkingCitizen = null;
        private Dog playerDog = null;

        protected override void Start()
        {
            base.Start();
            playerStats = GetComponent<NewPlayerStats>();
            playerStats.onStatCritical += OnStatCritical;
            TimeManager.Instance.onNewDay += OnNewDay;
            
        }

        private void OnNewDay(int day)
        {
            beliefs.RemoveState("HasWorkedToday");
        }

        private void OnStatCritical(string stat, float value)
        {
            if (BuildingManager.Instance != null && NotificationManager.Instance != null)
            {
                if (stat == "Hunger")
                {
                    if (!BuildingManager.Instance.IsBuildingConstructed(Buildings.BuildingData.BuildingType.SUPERMARKET))
                    {
                        NotificationManager.Instance.Warning("No supermarket", "Player is hungry and has no ingredients, but supermarket hasn't been built yet.");
                        return;
                    }
                }
            }
        }

        public void Setup()
        {
            SetInitialGoals();
        }

        private void SetInitialGoals()
        {
            goals.Clear();

            Goal g1 = new Goal("Eat", 1, false);
            goals.Add(g1, 1);

            Goal g2 = new Goal("Rest", 1, false);
            goals.Add(g2, 1);

            Goal g3 = new Goal("DoSports", 1, false);
            goals.Add(g3, 1);

            Goal g4 = new Goal("Work", 1, false);
            goals.Add(g4, 2);

            Goal g6 = new Goal("WaitForNurse", 1, true);
            goals.Add(g6, 4);
                

            //beliefs.AddState("HasIngredients", true);
        }



        public void StartTalk(Citizen citizen)
        {
            talkingCitizen = citizen;
        }
        public void StopTalking()
        {
            talkingCitizen = null;
        }
        public bool IsTalking()
        {
            return talkingCitizen != null;
        }
    }
}
