using Lore.Game.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public int MaximumDiseasesActive = 2;
        [Range(0f, 50f)] public float CureHealthGain = 40f;
        [SerializeField] private List<Disease> diseases = new List<Disease>();
        public int TotalDiseasesCured { get; private set; }

        public bool IsSick { get => currentDiseases.Count > 0; }
        public int ActiveDiseases { get => currentDiseases.Count; }

        [Header("Settings")]
        [SerializeField] private bool DebugText = false;
        [SerializeField] private bool UseRandomDisease = true;
        [SerializeField] private float FatigueProbabilityMultiplier = 2f;
        [SerializeField] private float HungerProbabilityMultiplier = 1.4f;
        [SerializeField] private bool NotifyOnSickess;
        [SerializeField] private bool GiveRandomSicknessOnStart;
 
        // Events
        public Action<Disease> onNewDisease;
        public Action onCured;

        [SerializeField] private List<Disease> currentDiseases = new List<Disease>();
        private GamePlayer player;
        private NewPlayerStats playerStats;
        private float fatigueTimestamp;
        private bool isInTiredState = false;

        public override void Start()
        {
            player = FindFirstObjectByType<GamePlayer>();
            playerStats = player.GetComponent<NewPlayerStats>();

            playerStats.onStatCritical += OnStatCritical;
            playerStats.onStatCriticalExit += OnStatCriticalExit;
            TimeManager.Instance.onNewDay += OnNewDay;
            Managers.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            StartCoroutine(StartDiseaseRun());
            base.Start();
        }

        private IEnumerator SetInitialDisease(Disease d)
        {
            yield return new WaitUntil(() => player.IsActive);
            StartDisease(d);
        }

        private void OnGameStateChanged(GameManager.GameState state1, GameManager.GameState state2)
        {
            if (state2 == GameManager.GameState.PLAYING)
            {
                if (GiveRandomSicknessOnStart)
                {
                    System.Random random = new System.Random();
                    int index = random.Next(diseases.Count);
                    Disease d = diseases[index];
                    StartCoroutine(SetInitialDisease(d));
                }
            }
        }

        private void OnNewDay(int newDay)
        {
            if (currentDiseases.Count < MaximumDiseasesActive)
            {
                Disease disease = CalculateSickness();
                if (disease != null)
                {
                    onNewDisease?.Invoke(disease);
                    if (NotifyOnSickess)
                    {
                        NotificationManager.Instance.Warning("Player new disease", $"Player is sick for {disease.Name}. Health drain rate = {disease.HealthLossRatePerSec}, Active in {disease.ActiveAfterSeconds} seconds");
                    }
                    StartDisease(disease);
                }
            }
            
        }
        private void OnStatCritical(string stat, float value)
        {
            if (stat == "Fatigue")
            {
                isInTiredState = true;
            }
        }
        private void OnStatCriticalExit(string stat)
        {
            if (stat == "Fatigue"){
                isInTiredState = false;
            }
        }

        private IEnumerator StartDiseaseRun()
        {
            while (true)
            {
                if (currentDiseases.Count > 0)
                {
                    foreach(Disease disease in currentDiseases)
                    {
                        if (disease.IsActive)
                            playerStats.TakeDamage(disease.HealthLossRatePerSec, NewPlayerStats.DamageReason.DISEASE);
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
        private void StartDisease(Disease disease)
        {
            StartCoroutine(StartDiseaseCo(disease));
        }
        private IEnumerator StartDiseaseCo(Disease disease)
        {
            currentDiseases.Add(disease);
            if (currentDiseases.Count == 1 && !player.beliefs.HasState("NeedsCures"))
            {
                player.beliefs.ModifyState("NeedsCures", true);
                //player.AddGoal("WaitForNurse", 4, true);
                player.AddGoal("GetTreated", 5, true);
            }
            yield return new WaitForSeconds(disease.ActiveAfterSeconds);
            disease.Activate();
        }
        public void CureAll()
        {
            TotalDiseasesCured += currentDiseases.Count;
            currentDiseases.Clear();
            player.GetComponent<NewPlayerStats>().GainHealth(CureHealthGain);
        }
        public Disease CalculateSickness()
        {
            System.Random rand = new System.Random();
            float Fmodifier = isInTiredState ? FatigueProbabilityMultiplier : 1f;
            float Hmodifier = isInTiredState ? HungerProbabilityMultiplier : 1f;
            for (int i=0; i<diseases.Count; i++)
            {
                //float r = UnityEngine.Random.Range(0f, 1f);
                float r = (float)rand.NextDouble();
                if (DebugText)
                    Debug.Log($"[SicknessMan.] Disease: {diseases[i].Name}, r: {r}, prob: {diseases[i].DailyProbabilityOfEncounter * Fmodifier * Hmodifier},  got it: {r <= diseases[i].DailyProbabilityOfEncounter * Fmodifier * Hmodifier}");
                if (r <= diseases[i].DailyProbabilityOfEncounter * Fmodifier * Hmodifier)
                {
                    return diseases[i];
                }
            }
            return null;
        }

    }

}
