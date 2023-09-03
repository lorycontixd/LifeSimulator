using GOAP;
using Lore.Game.Managers;
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
        public bool IsReturningHome { get; private set; }


        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            _defaultSpeed = agent.speed;
            animator = GetComponent<Animator>();
            bot = GetComponent<Bot>();
            player = FindFirstObjectByType<GamePlayer>();
            TimeManager.Instance.onDayPartChange += OnDayPartChange;
            TimeManager.Instance.onNewDay += OnNewDay;


            GoToRest();
            SetupFSM();

            StartCoroutine(RunFSM());
        }
        private void Update()
        {
            if (state == DogState.DEMANDINGWALK)
            {
                demandTimestamp += Time.deltaTime;
            }
            if (TimeManager.Instance.TimeSinceStart - lastEat > hungryAfterSeconds && !IsHungry)
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

            idle.enterActions.Add(OnIdleEnter);
            idle.exitActions.Add(OnIdleExit);
            demandWalk.enterActions.Add(OnDemandWalk);
            demandWalk.exitActions.Add(OnDemandExit);
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

            fsm = new FSM(idle);
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
            if (timesWalkedToday < maxWalksPerDay && state == DogState.IDLE && !GWorld.Instance.GetWorld().HasState("DogWantsWalk"))
            {
                float prob = CalculateWalkDemandProbability();
                if (UnityEngine.Random.Range(0f, 1f) < prob)
                {
                    WantsWalk();
                }
            }
            if (TimeManager.Instance.TimeSinceStart - lastEat > deathNoEatSeconds)
            {
                onDeath?.Invoke();
            }
        }

        private float CalculateWalkDemandProbability()
        {
            if (timesWalkedToday == 0)
            {
                return 0.9f;
            }else if(timesWalkedToday == 1)
            {
                return 0.5f;
            }
            else
            {
                return 0.3f;
            }
        }

        public void WantsWalk(bool notify = true)
        {
            GWorld.Instance.GetWorld().AddState("DogWantsWalk", true);
            this.player.AddGoal("WalkDog", 2, true);
            _wantsToWalk = true;
            if (notify)
                NotificationManager.Instance.Info("Dog info", "Your dog wants to be taken for a walk!");
        }
        public void StartDogWalk()
        {
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
            IsReturningHome = true;
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

        public void OnIdleEnter()
        {
            _speed = -1f;
            animator.SetFloat("Speed", _speed);
            animator.SetInteger("Idle", 0);
        }
        public void OnIdleExit()
        {
        }
        private void OnDemandWalk()
        {
            state = DogState.DEMANDINGWALK;
            _speed = -1f;
            animator.SetFloat("Speed", _speed);
            animator.SetInteger("Idle", 0);
        }
        private void OnDemandExit()
        {
        }
        private void OnWalkStarted()
        {
            state = DogState.WALKING;
            _playerStartedWalk = false;
            _wantsToWalk = false;
            bot.SetBotAction(Bot.BotAction.SEEK, player.gameObject);
            _speed = _defaultSpeed;
            animator.SetFloat("Speed", _speed);
        }
        private void OnWalkCompleted()
        {
            _playerCompletedWalk = false;
            state = DogState.IDLE;
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
            lastEat = TimeManager.Instance.TimeSinceStart;
            state = DogState.IDLE;
        }
        #endregion


        public void Reset()
        {
            OnWalkCompleted();
        }

    }
}
