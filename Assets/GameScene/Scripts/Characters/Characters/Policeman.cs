using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Lore.Game.Characters
{
    public class Policeman : Character
    {
        #region Enums
        public enum PolicemanState
        {
            PATROLLING,
            ALERT,
            DANGER
        }
        [Serializable]
        public struct StateSpeed
        {
            public PolicemanState state;
            public float speed;
        }
        [Serializable]
        public struct TorchIntensityDay
        {
            public DayPart dayPart;
            public float intensity;
        }
        #endregion

        public bool IsActive { get; private set; } = false;
        public float alarmDurationToDanger = 1f;
        public float fsmUpdateRateSec = 0.4f;
        public float dangerScanRateSec = 0.2f;
        public float capturedAfterSecsInDanger = 3f;
        public float turnTowardsCenterDuration = 2f;
        public float wrongDirectionCheckRange = 10f;
        public List<StateSpeed> speeds = new List<StateSpeed>();
        [SerializeField] private Transform patrolWaypoints;


        public PolicemanState state { get; private set; } = PolicemanState.PATROLLING;
        public bool IsChasing { get; private set; }

        [Header("Torch light")]
        [SerializeField] private List<TorchIntensityDay> torchIntensities;
        [SerializeField] private Light torchLight;
        [SerializeField] private Color normalLightColour;
        [SerializeField] private Color alertLightColour;
        [SerializeField] private Color dangerLightColour;


        private PolicemanState lastState;
        public GameObject dangerousObject { get; private set; } = null;
        // Events
        public Action<Policeman, GameObject, DateTime> onAlert;
        public Action<Policeman, DateTime> onAlertDismiss;
        public Action<Policeman, GameObject, DateTime> onSpotted;
        public Action<Policeman, GameObject, DateTime> onCapturedDanger;

        // Privates
        private Vector3 facingDirection;
        private GamePlayer PlayerInSight = null;
        private PoliceStation.CommunicationMessage lastMessageReceived = PoliceStation.CommunicationMessage.NONE;
        private PoliceStation.CommunicationData lastReceivedData = null;
        private FSM fsm;
        private Bot bot;
        private NavMeshAgent agent;
        private FieldOfFiew3D fov;
        private Transform mapCenter;
        private int currentWaypointIndex;
        private float alertStartTime;
        private float alertDuration;
        private bool reachedPlayerDuringDanger = false;
        private float captureTimestamp = 0f;
        private bool isLightOn = false;
        private int boundaryHitCount = 0;
        private bool isLerpingTowardsCenter = false;
        private float lerpingTime = 0;

        private void Start()
        {
            bot = GetComponent<Bot>();
            bot.SetBotAction(Bot.BotAction.NONE);
            fov = GetComponent<FieldOfFiew3D>();
            agent = GetComponent<NavMeshAgent>();
            if (torchLight == null)
            {
                torchLight = GetComponentInChildren<Light>();
            }
            mapCenter = GameObject.FindGameObjectWithTag("MapCenter").transform;
            TimeManager.Instance.onDayPartChange += OnDayPartChange;
            Lore.Game.Managers.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

            //PoliceStation.Instance.RegisterAgent(this);
            if (fov != null && torchLight != null)
            {
                torchLight.range = fov.distance;
                torchLight.spotAngle = fov.angle * 2f;
                torchLight.enabled = true;
                isLightOn = true;
                SetDayLightIntensity(TimeManager.Instance.CurrentDayPart);
                //torchLight.intensity = 400f;
            }
        }

        private void OnGameStateChanged(Managers.GameManager.GameState state1, Managers.GameManager.GameState state2)
        {
            if (state2 == Managers.GameManager.GameState.PLAYING && !IsActive)
            {
                SetupFSM();
                StartCoroutine(CorrectFacingDiraction());
                StartCoroutine(RunFSM());
                IsActive = true;
            }
        }

        private void OnDayPartChange(DayPart part1, DayPart part2)
        {
            SetDayLightIntensity(part2);
        }

        private void SetDayLightIntensity(DayPart daypart)
        {
            bool hasDay = torchIntensities.Any(t => t.dayPart == daypart);
            if (hasDay)
                torchLight.intensity = torchIntensities.FirstOrDefault(t => t.dayPart == daypart).intensity;
            else
                torchLight.intensity = 5f;
        }

        private void Update()
        {
            if (IsActive)
            {
                facingDirection = Quaternion.AngleAxis(transform.rotation.y, Vector3.up) * transform.forward;
                if (isLerpingTowardsCenter)
                {
                    Vector3 targetDir = mapCenter.position - transform.position;
                    targetDir.y = transform.position.y;
                    Quaternion targetQuaternion = Quaternion.LookRotation(targetDir);

                    var factor = lerpingTime / turnTowardsCenterDuration;
                    Quaternion lerped = Quaternion.Lerp(transform.rotation, targetQuaternion, factor);
                    transform.rotation = lerped;
                    lerpingTime += Time.deltaTime;
                    if (lerpingTime > turnTowardsCenterDuration)
                    {
                        isLerpingTowardsCenter = false;
                        lerpingTime = 0;
                    }
                }
            }
            
        }
        private void SetupFSM()
        {
            FSMState patrol = new FSMState();
            FSMState alert = new FSMState();
            FSMState danger = new FSMState();

            alert.enterActions.Add(OnAlertEnter);
            alert.stayActions.Add(OnAlertStay);
            alert.exitActions.Add(OnAlertExit);
            patrol.enterActions.Add(OnPatrolEnter);
            danger.enterActions.Add(OnDangerEnter);
            danger.stayActions.Add(OnDangerStay);
            danger.exitActions.Add(OnDangerExit);


            FSMTransition patrolToAlert = new FSMTransition(SuspiciousActivity);
            FSMTransition alertToDanger = new FSMTransition(AlertConfirmed);
            FSMTransition alertToPatrol = new FSMTransition(AlertDismissed);
            FSMTransition dangerToPatrol = new FSMTransition(StopChase);

            patrol.AddTransition(patrolToAlert, alert);
            alert.AddTransition(alertToDanger, danger);
            alert.AddTransition(alertToPatrol, patrol);
            danger.AddTransition(dangerToPatrol, patrol);


            fsm = new FSM(patrol);
            fsm.RegisterStates(new FSMState[] { patrol, alert, danger });
            StartCoroutine(ScanDangers());
        }
        private IEnumerator RunFSM() {
            while (true)
            {
                fsm.Update();
                yield return new WaitForSeconds(fsmUpdateRateSec);
            }
        }
        private IEnumerator ScanDangers()
        {
            while (true)
            {
                PlayerInSight = CheckPlayerInSight();
                yield return new WaitForSeconds(dangerScanRateSec);
            }
        }
        private void ChangeState(PolicemanState newState)
        {
            lastState = state;
            state = newState;
        }

        private GamePlayer CheckPlayerInSight()
        {
            if (fov == null) { return null; }
            if (fov.ObjectsInView == null) { return null; }
            foreach(GameObject obj in fov.ObjectsInView)
            {
                GamePlayer player = obj.GetComponent<GamePlayer>();
                if (player != null)
                {
                    return player;
                }
            }
            return null;
        }
        private IEnumerator DamagerPlayerCo(NewPlayerStats stats)
        {
            while (true)
            {
                stats.TakeDamage(50f, NewPlayerStats.DamageReason.ATTACK);
                yield return new WaitForSeconds(0.8f);
            }
        }

        private IEnumerator CorrectFacingDiraction()
        {
            while (true)
            {
                CheckBoundaryCollider();
                yield return new WaitForSeconds(0.5f);
            }
        }
        private void CheckBoundaryCollider()
        {
            RaycastHit hit;
            Ray r = new Ray(transform.position, facingDirection);
            if (state == PolicemanState.PATROLLING && !isLerpingTowardsCenter)
            {
                if (Physics.Raycast(r, out hit, wrongDirectionCheckRange))
                {
                    if (hit.collider.tag == "BoundaryCollider")
                    {
                        boundaryHitCount++;
                        if (boundaryHitCount > 10)
                        {
                            bot.SetBotAction(Bot.BotAction.NONE);
                            isLerpingTowardsCenter = true;
                        }
                    }
                    else
                    {
                        boundaryHitCount = 0;
                        bot.SetBotAction(Bot.BotAction.WANDER);
                    }
                }
                else
                {
                    boundaryHitCount = 0;
                    bot.SetBotAction(Bot.BotAction.WANDER);
                }
            }
            
        }

        #region Station Communication
        public void ReceivedAlert(PoliceStation.CommunicationMessage msg, PoliceStation.CommunicationData data)
        {
            lastMessageReceived = msg;
            lastReceivedData = data;

        }
        #endregion

        #region Conditions
        public bool SuspiciousActivity()
        {
            if (lastMessageReceived == PoliceStation.CommunicationMessage.ALERT)
            {
                dangerousObject = lastReceivedData.Object;
                return true;
            }
            if (PlayerInSight == null)
            {
                return false;
            }
            dangerousObject = PlayerInSight.gameObject;
            return PlayerInSight.IsStealing;
        }
        public bool AlertConfirmed()
        {
            if (lastMessageReceived == PoliceStation.CommunicationMessage.DANGER) { return true; }
            return alertDuration >= alarmDurationToDanger && PlayerInSight;
        }

        public bool AlertDismissed()
        {
            if (alertDuration < alarmDurationToDanger) {return false; }
            bool isPlayerInSight = CheckPlayerInSight();
            if (!isPlayerInSight) { return true; }
            bool isStealing = PlayerInSight.IsStealing;
            if (!isStealing) { return false; }
            return true;
        }
        public bool StopChase()
        {
            bool isArrested = false;
            bool lostDanger = false;
            return isArrested || lostDanger;
        }
        #endregion

        #region Actions
        public void OnPatrolEnter()
        {
            ChangeState(PolicemanState.PATROLLING);
            /*if (lastState != PolicemanState.PATROLLING)
            {
                onAlertDismiss?.Invoke(this, DateTime.Now);
            }*/
            bot.SetBotAction(Bot.BotAction.WANDER);
            agent.speed = speeds.FirstOrDefault(s => s.state == state).speed;
            if (torchLight != null)
            {
                torchLight.color = normalLightColour;
            }
        }
        public void OnPatrolStay()
        {
        }
        public void OnAlertEnter()
        {
            SendAlert();
            if (lastMessageReceived != PoliceStation.CommunicationMessage.ALERT)
                onAlert?.Invoke(this, dangerousObject, DateTime.Now);
            if (torchLight != null)
            {
                torchLight.color = alertLightColour;
            }
        }
        private void SendAlert()
        {
            alertStartTime = TimeManager.Instance.TimeSinceStart;
            ChangeState(PolicemanState.ALERT);
            bot.SetBotAction(Bot.BotAction.SEEK, dangerousObject);
            agent.speed = agent.speed / 2.1f;
            agent.speed = speeds.FirstOrDefault(s => s.state == state).speed;
        }
        public void OnAlertStay()
        {
            alertDuration = TimeManager.Instance.TimeSinceStart - alertStartTime;
        }
        public void OnAlertExit()
        {

        }

        public void OnDangerEnter()
        {
            ChangeState(PolicemanState.DANGER);
            bot.SetBotAction(Bot.BotAction.SEEK, dangerousObject);
            agent.speed = speeds.FirstOrDefault(s => s.state == state).speed;
            if (lastMessageReceived != PoliceStation.CommunicationMessage.DANGER)
                onSpotted?.Invoke(this, dangerousObject, DateTime.Now);
            if (torchLight != null)
            {
                torchLight.color = dangerLightColour;
            }
        }
        public void OnDangerStay()
        {
            if (dangerousObject != null)
            {
                GamePlayer player = dangerousObject.GetComponent<GamePlayer>();
                if (player != null)
                {
                    NewPlayerStats stats = player.GetComponent<NewPlayerStats>();
                    if (stats != null)
                        StartCoroutine(DamagerPlayerCo(stats));
                }
            }
        }
        public void OnDangerExit()
        {

        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + facingDirection * wrongDirectionCheckRange);
        }
    }

}
