using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Lore.Game.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(Animator))]
    public class Dog : Character
    {
        public enum DogState
        {
            IDLE,
            DEMANDINGWALK,
            WALKING,
            EATING,
            DEAD
        }


        [SerializeField] private Transform restPosition;
        [SerializeField] private int maxWalksPerDay = 2;
        [SerializeField] private float eatDuration = 5f;
        [SerializeField] private float hungryAfterSeconds = 70f;
        [SerializeField] private float deathNoEatSeconds = 250f;
        [SerializeField] private float fsmUpdateRateSec = 0.7f;

        public bool IsHungry { get; private set; } = false;
        public bool IsActive { get; private set; } = true;
        public float GetDemandTime
        {
            get
            {
                if (state == DogState.DEMANDINGWALK)
                {
                    return demandTimestamp;
                }
                else
                {
                    return -1f;
                }
            }
        }
        public DogState state = DogState.IDLE;

        public Action onWalkDemand;
        public Action onWalkCompleted;
        public Action onDeath;

        private FSM fsm;
        private NavMeshAgent agent;
        private Animator animator;
        private Bot bot;
        private GamePlayer player;
        private float demandTimestamp = 0f;
        private float _defaultSpeed;
        private float _speed;
        private int timesWalkedToday = 0;
        private float lastEat = 0f;
        private bool _isEating = false;
        private bool _wantsToWalk = false;
        private bool _playerStartedWalk = false;
        private bool _playerCompletedWalk = false;
        private bool _hasFood = false;
        private bool _isReturningHome;


        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            _defaultSpeed = agent.speed;
            animator = GetComponent<Animator>();
            bot = GetComponent<Bot>();
            player = FindFirstObjectByType<GamePlayer>();
            TimeManager.Instance.onDayPartChange += OnDayPartChange;
            TimeManager.Instance.onNewDay += OnNewDay;

            SetupFSM();
        }
        private void Update()
        {
            if ( state == DogState.WALKING && _isReturningHome)
            {
                if (Vector3.Distance(agent.transform.position, restPosition.position) < 1.5f)
                {
                    // Arrived rest place after walk
                    animator.SetFloat("Speed", 0f);
                    animator.SetInteger("Idle", 0);
                    this.bot.SetBotAction(Bot.BotAction.NONE);
                }
            }
            if (state == DogState.DEMANDINGWALK)
            {
                demandTimestamp += Time.deltaTime;
            }
            if (Time.realtimeSinceStartup - lastEat > hungryAfterSeconds)
            {
                IsHungry = true;
            }
        }
        private void SetupFSM()
        {
            FSMState idle = new FSMState();
            FSMState demandWalk = new FSMState();
            FSMState walk = new FSMState();
            FSMState eat = new FSMState();
            FSMState dead = new FSMState();

            demandWalk.enterActions.Add(OnDemandWalk);
            walk.enterActions.Add(OnWalkStarted);
            walk.exitActions.Add(OnWalkCompleted);
            eat.enterActions.Add(OnEatStart);
            eat.exitActions.Add(OnEatComplete);

            FSMTransition idleToDemand = new FSMTransition(WantsToWalk);
            FSMTransition demandToWalk = new FSMTransition(PlayerStartedWalk);
            FSMTransition walkToIdle = new FSMTransition(PlayerFinishedWalk);
            FSMTransition idleToEat = new FSMTransition(CanEat);
            FSMTransition eatToIdle = new FSMTransition(StopEat);

            idle.AddTransition(idleToDemand, demandWalk);
            demandWalk.AddTransition(demandToWalk, walk);
            walk.AddTransition(walkToIdle, idle);
            idle.AddTransition(idleToEat, eat);
            eat.AddTransition(eatToIdle, idle);

            GoToRest();
            fsm = new FSM(idle);
            StartCoroutine(RunFSM());
            StartCoroutine(TestWantsWalk());
        }
        private IEnumerator RunFSM()
        {
            while (IsActive)
            {
                fsm.Update();
                yield return new WaitForSeconds(fsmUpdateRateSec);
            }
        }
        private IEnumerator TestWantsWalk()
        {
            yield return new WaitForSeconds(3f);
            WantsWalk();
        }

        private void OnNewDay(int obj)
        {
            timesWalkedToday = 0;
        }

        private void OnDayPartChange(DayPart part1, DayPart part2)
        {
            /*if (timesWalkedToday < maxWalksPerDay && state != DogState.DEMANDINGWALK)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.7f)
                {
                    WantsWalk();
                }
            }*/
            if (Time.realtimeSinceStartup - lastEat > deathNoEatSeconds)
            {
                onDeath?.Invoke();
            }
        }

        public void WantsWalk()
        {
            Debug.Log($"Dog wants to walk!!");
            GWorld.Instance.GetWorld().AddState("DogWantsWalk", true);
            this.player.AddGoal("WalkDog", 1, true);
            _wantsToWalk = true;
        }
        public void StartDogWalk()
        {
            Debug.Log($"Player started dog walk!");
            _wantsToWalk = false;
            _playerStartedWalk = true;
            GWorld.Instance.GetWorld().RemoveState("DogWantsWalk");
        }
        public void EndDogWalk()
        {
            _playerCompletedWalk = true;
        }
        public void GiveFood()
        {
            _hasFood = true;
        }
        public void GoToRest()
        {
            this.bot.SetBotAction(Bot.BotAction.SEEK, restPosition.gameObject);
        }
        public void ReturningHome()
        {
            _isReturningHome = true;
        }

        #region Conditions
        private bool WantsToWalk()
        {
            return _wantsToWalk;
        }
        private bool PlayerStartedWalk()
        {
            return _playerStartedWalk;
        }
        private bool PlayerFinishedWalk()
        {
            return _playerCompletedWalk;
        }
        private bool CanEat()
        {
            return _hasFood && IsHungry;
        }
        private bool StopEat()
        {
            return _isEating == false && state == DogState.EATING;
        }
        #endregion

        #region Actions
        private void OnDemandWalk()
        {
            state = DogState.DEMANDINGWALK;
        }
        private void OnWalkStarted()
        {
            state = DogState.WALKING;
            _playerStartedWalk = false;
            _speed = _defaultSpeed;
            animator.SetFloat("Speed", _speed);
            bot.SetBotAction(Bot.BotAction.SEEK, player.gameObject);
        }
        private void OnWalkCompleted()
        {
            _playerCompletedWalk = false;
            GoToRest();
        }
        private void OnEatStart()
        {
            state = DogState.EATING;
            StartCoroutine(Eat());
        }
        private IEnumerator Eat()
        {
            _isEating = true;
            yield return new WaitForSeconds(eatDuration);
            _isEating = false;

        }
        private void OnEatComplete()
        {
            lastEat = Time.realtimeSinceStartup;
            state = DogState.IDLE;
        }
        #endregion

    }
}
