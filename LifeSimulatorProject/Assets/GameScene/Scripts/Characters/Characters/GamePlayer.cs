using GOAP;
using Lore.Game.Managers;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using NotificationManager = Lore.Game.Managers.NotificationManager;

namespace Lore.Game.Characters
{
    public class GamePlayer : GAgent
    {
        #region ENums
        public enum PlayerState
        {
            STARTING,
            ALIVE,
            OUTLAW,
            DEAD
        }
        #endregion

        public float BaseGoalsUpdateRateSec = 0.9f;
        [Range(0f,1f)] public float PlanRefusalProbabilityWhenSad = 0.15f;

        public PlayerState playerState { get; private set; }
        public bool IsActive { get; private set; } = false;
        public bool IsStealing { get; private set; } = false;

        NewPlayerStats playerStats;
        PlayerHouseRegion houseRegion;
        private Citizen talkingCitizen = null;
        private Dog playerDog = null;
        private bool isPaused = false;
        private float setgoalsTimestamp = 0f;
        private Goal eatGoal = null;
        private Goal sleepGoal = null;
        private Goal interactGoal = null;
        private float baseGoalTimestamp = 0f;
        private PlayerState lastPlayerState;

        //private Citizen interactingNPC;


        protected override void Start()
        {
            lastPlayerState = PlayerState.STARTING;
            base.Start();
            beliefs.AddState("LastAction", "");
            playerDog = FindFirstObjectByType<Dog>();
            houseRegion = GameObject.FindGameObjectWithTag("PlayerHouse").GetComponentInChildren<PlayerHouseRegion>();
            houseRegion.onPlayerEnterHouse.AddListener(OnPlayerEnterHouse);
            houseRegion.onPlayerExitHouse.AddListener(OnPlayerExitHouse);
            playerStats = GetComponent<NewPlayerStats>();
            playerStats.onStatCritical += OnStatCritical;
            playerStats.onHealthChanged += OnHealthChanged;
            TimeManager.Instance.onNewDay += OnNewDay;
            BuildingManager.Instance.onLevelUpgrade.AddListener(OnCityLevelUpgrade);
            Lore.Game.Managers.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

            this.onPlanFound += OnPlanFound;
        }

        private void OnCityLevelUpgrade(int newLevel)
        {
            DurationModifierPerc = Mathf.Clamp((float)newLevel * 2f, 0f, DurationModifierPercentageMax);
        }

        private void OnHealthChanged(float oldvalue, float newValue)
        {
            if (newValue <= 50f && !beliefs.HasState("NeedsCures")){
                beliefs.ModifyState("NeedsCures", true);
                AddGoal("GetTreated", 5, true);
            }
        }

        private void OnPlanFound(string obj)
        {
            /*if (beliefs.HasState("IsSad"))
            {
                if (UnityEngine.Random.Range(0f,1f) < PlanRefusalProbabilityWhenSad)
                {
                    Managers.NotificationManager.Instance.Info("Plan refused", "Player is sad and refused the proposed plan.");
                    StartCoroutine(DeactivatePlanSearch(5f));
                    return;
                }
            }*/
        }

        private IEnumerator DeactivatePlanSearch(float duration)
        {
            CanSearchPlan = false;
            yield return new WaitForSeconds(duration);
            CanSearchPlan = true;
        }

        public void Setup()
        {
            SetInitialGoals();
            IsActive = true;
            SetPlayerState(PlayerState.ALIVE);
        }

        private void OnGameStateChanged(Managers.GameManager.GameState state1, Managers.GameManager.GameState state2)
        {
            if (state2 == Managers.GameManager.GameState.PLAYING)
            {
                Debug.Log($"[GamePlayer] GameManager playing => Starting agent");
                Setup();
            }
        }

        private void OnPlayerExitHouse()
        {
            if (beliefs.HasState("DogFollowing"))
            {
                if (playerDog != null)
                {
                    if (playerDog.state == Dog.DogState.WALKING && playerDog.IsReturningHome)
                    {
                        playerDog.EndDogWalk();
                    }
                }
            }
        }

        private void OnPlayerEnterHouse()
        {

        }

        private void Update()
        {
            if (IsActive)
            {
                if (Time.time > baseGoalTimestamp)
                {
                    UpdateBaseGoals();
                    baseGoalTimestamp = Time.time + BaseGoalsUpdateRateSec;
                }
                CheckInteractionStatus();
            }

        }

        private void CheckInteractionStatus()
        {
            if (FriendshipManager.Instance != null)
            {
                if (FriendshipManager.Instance.CountMoodyFriends() > 0 )
                {
                    if (!beliefs.HasState("CanInteract"))
                    {
                        this.beliefs.AddState("CanInteract", true);
                    }
                }
                else
                {
                    if (beliefs.HasState("CanInteract"))
                    {
                        this.beliefs.RemoveState("CanInteract");
                    }
                }
            }
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

        private void SetInitialGoals()
        {
            goals.Clear();

            eatGoal = new Goal("Eat", 1, false);
            goals.Add(eatGoal, 1);

            sleepGoal = new Goal("Rest", 1, false);
            goals.Add(sleepGoal, 1);

            Goal g3 = new Goal("DoSports", 1, false);
            goals.Add(g3, 2);

            Goal g4 = new Goal("Work", 1, false);
            goals.Add(g4, 3);

            Goal g5 = new Goal("GoHome", 1, false);
            goals.Add(g5, 1);

            interactGoal = new Goal("Interact", 1, false);
            goals.Add(interactGoal, 2);
            // Nurse goal added when gets sick
            //Goal g6 = new Goal("WaitForNurse", 1, true);
            //goals.Add(g6, 4);
        }
        public void PrintGoals()
        {
            int i = 0;
            foreach(var goal in goals)
            {
                Debug.Log($"Goal{i} ==> {goal.Key.sGoals.Last().Key}");
                i++;
            }
        }
        private void UpdateBaseGoals()
        {
            goals[eatGoal] = (int)playerStats.Hunger;
            goals[sleepGoal] = (int)playerStats.Fatigue;
            goals[interactGoal] = (int)(playerStats.Sadness / playerStats.MaxSadness * 6f);
        }

        public void SetPlayerState(PlayerState newstate)
        {
            lastPlayerState = playerState;
            playerState = newstate;
        }

        public override void PauseAgent()
        {
            playerStats.Activate(false);
            base.PauseAgent();
            isPaused = true;
        }
        public override void ResumeAgent()
        {
            playerStats.Activate(true);
            base.ResumeAgent();
            isPaused = false;
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

        public void SetStealing(bool isStealing)
        {
            this.IsStealing = isStealing;
            SetPlayerState((isStealing) ? PlayerState.OUTLAW : PlayerState.ALIVE);
        }
    }
}
