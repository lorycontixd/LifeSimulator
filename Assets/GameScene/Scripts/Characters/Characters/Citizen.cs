using Lore.Game.Buildings;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static NPC;
using UnityEngine.EventSystems;

namespace Lore.Game.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WaypointWandering))]
    [RequireComponent(typeof(Bot))]
    public class Citizen : Character
    {
        #region Enums
        public enum CitizenState
        {
            WANDER,
            TALK,
            REST,
            WORK
        }
        public enum WanderMode
        {
            STEERING,
            WAYPOINTS,
            BUILDINGS
        }
        #endregion

        // Settings
        [Header("Settings")]
        [SerializeField] private Personality personality;
        [SerializeField] private float playerScanRange;
        public WanderMode wanderMode;
        [SerializeField] private float probabilityOfTalking;
        [SerializeField] private float crossRoadSpeedReduction = 3f;
        [SerializeField] private float fsmUpdateRateSeconds = 0.8f;
        [SerializeField] private float arrivalDistance = 2f;
        public bool IsActive = true;


        [Header("Debug")]
        public bool DebugBuildings = false;
        public bool DebugText = false;
        [SerializeField, Tooltip("Set the buildings that the npc should go to if you want a deterministic behaviour. Must be greater than 2")] private List<Buildings.Building> buildings = new List<Buildings.Building>();

        // Events

        // Privates
        private NavMeshAgent agent;
        private Bot bot;
        private Friendship friendship;
        private Buildings.Building citizenHouse;
        private Buildings.Building citizenWork;
        private FSM fsm;
        private GamePlayer player;
        private float defaultAgentSpeed;
        private bool isPlayerNearby = false;
        private bool isPlayerScanActive = false;
        private bool isPlayersFriend;
        private bool isTalkingToPlayer;
        private float talkingDurationTimestamp;
        private float talkingCooldownTimestamp;
        private bool isTired;
        private WaypointWandering waypointWandering;
        private CitizenState state = CitizenState.WANDER;
        private Buildings.Building targetBuilding = null; // For building wandering
        private Buildings.Building lastBuilding = null; // Last building visited for building wandering
        private int TargetBuildingDebugIndex = 0;


        private IEnumerator Start()
        {
            bot = GetComponent<Bot>();
            if (friendship == null)
            {
                friendship = GetComponent<Friendship>();
            }
            waypointWandering = GetComponent<WaypointWandering>();
            agent = GetComponent<NavMeshAgent>();
            defaultAgentSpeed = agent.speed;
            yield return new WaitUntil(() => TimeManager.Instance.IsSetup);
            TimeManager.Instance.onDayPartChange += OnDayPartChange;
            yield return new WaitUntil(() => BuildingManager.Instance.IsSetup);
            citizenHouse = BuildingManager.Instance.GetRandomBuildingByType(BuildingData.BuildingType.HOUSE);
            citizenWork = BuildingManager.Instance.GetRandomBuildingByType(BuildingData.BuildingType.COMPANY);
            player = FindFirstObjectByType<GamePlayer>();
            SetupFSM();

            PopulationManager.Instance.AddCitizen(this);
            SetHouse(BuildingManager.Instance.GetRandomBuildingByType(BuildingData.BuildingType.HOUSE));
            SetWork(BuildingManager.Instance.GetRandomBuildingByType(BuildingData.BuildingType.COMPANY));
        }

        private void OnDayPartChange(DayPart oldPart, DayPart newPart)
        {
            //isTired = (newPart == DayPart.EVENING || newPart == DayPart.NIGHT);
        }

        private void Update()
        {
            if (isPlayerScanActive)
                ScanPlayer();
            if (DebugText)
            {
                Debug.Log($"Citizen{ID} state: {state}");
            }
            if (targetBuilding != null)
            {
                if (Vector3.Distance(transform.position, targetBuilding.transform.position) < arrivalDistance)
                {
                    StartCoroutine(ArriveAtBuilding());
                }
            }
        }
        private void SetupFSM()
        {
            FSMState wander = new FSMState();
            FSMState talk = new FSMState();
            FSMState resting = new FSMState();

            FSMTransition wanderToTalk = new FSMTransition(CanTalk);
            FSMTransition talkToWander = new FSMTransition(TalkEnded);
            FSMTransition wanderToRest = new FSMTransition(IsTired);
            FSMTransition restToWander = new FSMTransition(IsNotTired);

            talk.enterActions.Add(StartTalk);
            resting.enterActions.Add(GoHome);
            wander.enterActions.Add(OnWander);

            wander.AddTransition(wanderToTalk, talk) ;
            wander.AddTransition(wanderToRest, resting);
            talk.AddTransition(talkToWander, wander);
            resting.AddTransition(restToWander, wander);

            fsm = new FSM(wander);
            StartCoroutine(Run());
        }

        public void SetHouse(Buildings.Building house)
        {
            if (house.data.Type != BuildingData.BuildingType.HOUSE)
            {
                Debug.LogError($"[Citizen] Tried to set house, but building is not of type HOUSE");
                return;
            }
            this.citizenHouse = house;
        }
        public void SetWork(Buildings.Building work)
        {
            if (work.data.Type != BuildingData.BuildingType.COMPANY)
            {
                Debug.LogError($"[Citizen] Tried to set work, but building is not of type COMPANY");
                return;
            }
            this.citizenWork = work;
        }

        private void ScanPlayer()
        {
            if (player == null)
            {
                isPlayerNearby = false;
                return;
            }
            Collider[] colliders = Physics.OverlapSphere(transform.position, playerScanRange);
            isPlayerNearby = Vector3.Distance(player.transform.position, transform.position) < playerScanRange;
        }

        public void Interact(GamePlayer player, float duration)
        {
            StartCoroutine(InteractCo(player, duration));
        }
        private IEnumerator InteractCo(GamePlayer player, float duration)
        {
            this.agent.isStopped = true;
            LookAtPlayer(player);
            yield return new WaitForSeconds(duration);
            this.agent.isStopped = false;
            this.friendship.Interact();
        }
        public void LookAtPlayer(GamePlayer player)
        {
            Debug.Log($"Citizen looking at player!!");
            if (player == null)
            {
                player = FindFirstObjectByType<GamePlayer>();
            }
            this.transform.LookAt(player.transform);
        }



        #region Conditions
        public bool CanTalk()
        {
            if (!isPlayerNearby)
            {
                return false;
            }
            return !player.IsTalking();
        }
        public bool TalkEnded()
        {
            return isTalkingToPlayer == false;
        }
        public bool IsTired()
        {
            return isTired;
        }
        public bool IsNotTired()
        {
            return !isTired;
        }
        #endregion


        #region Actions
        public void OnEnterCrossroad()
        {
            //Debug.Log($"]NPC{ID}] Entered crossroad state!");
            agent.speed = agent.speed / crossRoadSpeedReduction;
        }
        public void OnExitCrossroad()
        {
            //behaviour = NPCBehaviour.CROSSROAD;
            //agent.speed = _baseSpeed;
        }
        public void StartTalk()
        {
            StartCoroutine(StartTalkCo(4f));
        }
        private IEnumerator StartTalkCo(float duration)
        {
            state = CitizenState.TALK;
            if (wanderMode == WanderMode.WAYPOINTS)
            {
                waypointWandering.Deactivate();
            }
            isTalkingToPlayer = true;
            player.StartTalk(this);
            yield return new WaitForSeconds(duration);
            player.StopTalking();
            isTalkingToPlayer = false;
        }
        public void GoHome()
        {
            Debug.Log($"GOing home targetCitizen");
            targetBuilding = null;
            bot.SetBotAction(Bot.BotAction.SEEK, citizenHouse.gameObject);
            state = CitizenState.REST;
            if (wanderMode == WanderMode.WAYPOINTS)
            {
                waypointWandering.Deactivate();
            }
        }
        public void OnWander()
        {
            state = CitizenState.WANDER;
            if (wanderMode == WanderMode.STEERING)
            {
                bot.SetBotAction(Bot.BotAction.WANDER);
            }
            else if (wanderMode == WanderMode.WAYPOINTS)
            {
                waypointWandering.Activate();
            }else if (wanderMode == WanderMode.BUILDINGS)
            {
                if (targetBuilding == null)
                {
                    if (IsActive)
                    {
                        if (DebugBuildings)
                        {
                            SetDebugDestination();
                        }
                        else
                        {
                            SetRandomDestination();
                        }
                    }
                }
                else
                {
                    SetDestination(targetBuilding.transform.position);
                }
            }

        }
        #endregion

        private IEnumerator Run()
        {
            while (true)
            {
                fsm.Update();
                yield return new WaitForSeconds(fsmUpdateRateSeconds);
            }
        }
        private IEnumerator ArriveAtBuilding(float duration = 4f, float randomRange = 0f)
        {
            if (DebugText)
                Debug.Log($"[Citizen->BuildingWander] Arrived at building {targetBuilding.data.Name}");
            IsActive = false;
            lastBuilding = targetBuilding;
            targetBuilding = null;
            if (randomRange < 0f || randomRange > duration)
            {
                Debug.LogWarning($"[NPC] Arrived at building -> random range for arrival is invalid. Setting random range to zero.");
                randomRange = 0f;
            }
            float noise = 0f;
            if (randomRange > 0)
            {
                noise = UnityEngine.Random.Range(-randomRange, randomRange);
            }
            yield return new WaitForSeconds(duration + noise);
            if (DebugBuildings)
            {
                TargetBuildingDebugIndex = (TargetBuildingDebugIndex + 1) % buildings.Count;
                SetDebugDestination();
            }
            else
            {
                SetRandomDestination();
            }
            IsActive = true;
        }

        private void SetRandomDestination()
        {
            Buildings.Building building = null;
            if ( lastBuilding == null)
            {
                building = BuildingManager.Instance.GetRandomBuilding();
            }
            else
            {
                building = BuildingManager.Instance.GetRandomBuildingExcept(new Buildings.Building[] { lastBuilding });
            }
            targetBuilding = building;
            agent.SetDestination(building.transform.position);
        }
        private void SetDebugDestination()
        {
            Buildings.Building building = buildings[TargetBuildingDebugIndex];
            targetBuilding = building;
            agent.SetDestination(building.transform.position);
        }
        private void SetDestination(Vector3 destination)
        {
            if (DebugText)
                Debug.Log($"[Citizen->BuildingWander] Setting destination {destination}");
            agent.SetDestination(destination);
        }
    }
}
