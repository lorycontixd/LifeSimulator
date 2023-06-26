using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerStats : MonoBehaviour
{
    #region Fatigue Calculation Mode
    [Serializable]
    private enum FatigueCalculationMode
    {
        TIME,
        PASSIVE
    }
    #endregion

    public float Hunger { get; private set; }
    public float Fatigue { get; private set; }
    public float Sadness { get; private set; }
    public Dictionary<string, float> Stats = new Dictionary<string, float>();

    [SerializeField] private FatigueCalculationMode fatigueCalculationMode;

    // Max values
    [SerializeField, Tooltip("Maximum fatigue allowed for the player. Lower numbers mean"), Range(0f, 10f)] float MaxFatigue = 5f;
    [SerializeField, Tooltip(""), Range(0f, 10f)] float MaxSadness = 5f;
    [SerializeField, Tooltip(""), Range(0f, 10f)] float MaxHunger = 5f;

    // Threshold values
    [SerializeField, Tooltip("")] private float FatigueThreshold = 3f;
    [SerializeField, Tooltip("")] private float HungerThreshold = 4f;

    // Passive stat change speed
    [SerializeField, Tooltip("How slow the passive fatigue builds up towards the maximum value. The higher the slower"), Range(1f, 10f)] float PassiveFatigueFactor = 2f; //
    [SerializeField, Tooltip("How slow the passive hunger builds up towards the maximum value. The higher the slower"), Range(1f, 10f)] float PassiveHungerFactor = 2f; //
    private float secondsToMaxFatigue = 120f;
    private float secondsToMaxHunger = 150f;
    private Dictionary<string, bool> HasSpawnedNotification = new Dictionary<string, bool>();

    // Props
    public bool IsActive { get; private set; } = true;

    // Events
    public Action<string, float> onStatCritical;

    private Dictionary<string, float> lastUpdatedStatFromStart = new Dictionary<string, float>();
    GAgent agent;



    private void Start()
    {
        this.agent = GetComponent<GAgent>();
        Setup();

        TimeManager.Instance.onDayPartChange += OnDayPartChange;
    }

    private void Setup()
    {
        Stats = new Dictionary<string, float>()
        {
            {"Hunger", 0f },
            {"Fatigue", 0f },
            {"Sadness", 0f }
        };
        this.agent.beliefs.AddState("Hunger", 0f);
        this.agent.beliefs.AddState("Fatigue", 0f);
        this.agent.beliefs.AddState("Sadness", 0f);
        ResetNotifications();
    }

    private void Update()
    {
        if (IsActive)
        {
            CalculateFatigue();
            CalculateHunger();
        }   
    }
    private void ResetNotifications()
    {
        HasSpawnedNotification = new Dictionary<string, bool>()
        {
            {"Hunger", false },
            {"Fatigue", false },
            {"Sadness", false}
        };
    }
    private void ResetNotification(string key)
    {
        HasSpawnedNotification[key] = false;
    }



    public void Eat()
    {
        this.Hunger = 0f;
        Stats["Hunger"] = 0f;
        this.agent.beliefs.ModifyState("Hunger", 0f);
        this.agent.beliefs.RemoveState("IsHungry");
        HasSpawnedNotification["Hunger"] = false;
    }

    public void Rest()
    {
        this.Fatigue = 0f;
        Stats["Fatigue"] = 0f;
        this.agent.beliefs.RemoveState("IsHungry");
        HasSpawnedNotification["Hunger"] = false;
    }

    private void OnDayPartChange(DayPart part1, DayPart part2)
    {
        if (fatigueCalculationMode == FatigueCalculationMode.TIME)
        {
            if (part2 == DayPart.EVENING)
            {
                this.Fatigue = 5f;
                this.agent.beliefs.AddState("IsTired", true);
            }
        }
    }

    private void CalculateFatigue()
    {
        if (fatigueCalculationMode == FatigueCalculationMode.PASSIVE)
        {
            float baseFatigue = (float)this.agent.beliefs.states["Fatigue"];
            float newFatigue = baseFatigue + MaxFatigue / secondsToMaxFatigue * PassiveFatigueFactor * Time.deltaTime;
            newFatigue = Mathf.Clamp(newFatigue, 0f, MaxFatigue);
            this.Fatigue = newFatigue;
            this.agent.beliefs.ModifyState("Fatigue", newFatigue);
            if (this.Fatigue > FatigueThreshold)
            {
                if (!this.agent.beliefs.states.ContainsKey("IsTired"))
                {
                    this.agent.beliefs.AddState("IsTired", true);
                }
                if (!HasSpawnedNotification["Fatigue"])
                {
                    onStatCritical?.Invoke("Fatigue", this.Fatigue);
                    HasSpawnedNotification["Fatigue"] = true;
                }
            }
            else
            {
                this.agent.beliefs.RemoveState("IsTired");
            }
        }
    }

    private void CalculateHunger()
    {
        float baseHunger = (float)this.agent.beliefs.states["Hunger"];
        float newHunger = baseHunger + MaxHunger / secondsToMaxHunger * PassiveHungerFactor * Time.deltaTime;
        newHunger = Mathf.Clamp(newHunger, 0f, MaxHunger);
        this.Hunger = newHunger;
        this.agent.beliefs.ModifyState("Hunger", newHunger);
        if (this.Hunger > HungerThreshold)
        {
            if (!this.agent.beliefs.states.ContainsKey("IsHungry"))
            {
                this.agent.beliefs.AddState("IsHungry", true);
            }
            if (!HasSpawnedNotification["Hunger"])
            {
                onStatCritical?.Invoke("Hunger", this.Hunger);
                HasSpawnedNotification["Hunger"] = true;
            }
        }
        else
        {
            this.agent.beliefs.RemoveState("IsHungry");
        }

    }
}
