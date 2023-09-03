using ES3Types;
using Lore.Game.Characters;
using Lore.Game.Managers;
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

    public enum DamageReason
    {
        UNKNOWN,
        HUNGER,
        DISEASE,
        ATTACK,
        CAR_ACCIDENT,
        USERINPUT,
        KILLED_BY_AUTHORITIES
    }

    #endregion

    [Range(50f, 200f)] public float MaxHealth = 100f;
    public float Health { get; private set; }

    public float Hunger { get; private set; }
    public float Fatigue { get; private set; }
    public float Sadness { get; private set; }
    public Dictionary<string, float> Stats = new Dictionary<string, float>();
    [SerializeField] private List<string> damagingStatsOnExtraCritical = new List<string>() { "Hunger" };
    [SerializeField] private float percentageDamagerPerFrame = 5f;
    [SerializeField] private FatigueCalculationMode fatigueCalculationMode;

    // Max values
    [SerializeField, Tooltip("Maximum fatigue allowed for the player. Lower numbers mean"), Range(0f, 10f)] float MaxFatigue = 5f;
    [SerializeField, Tooltip("")] public float MaxSadness;
    [SerializeField, Tooltip(""), Range(0f, 10f)] public float MaxHunger = 5f;

    // Threshold values
    [SerializeField, Tooltip("")] private float FatigueThreshold = 3f;
    [SerializeField, Tooltip("")] private float HungerThreshold = 4f;
    private float SadnessThreshold = -1;

    // Passive stat change speed
    [SerializeField, Tooltip("How slow the passive fatigue builds up towards the maximum value. The higher the slower"), Range(0.3f, 3f)] float PassiveFatigueFactor = 1.3f; //
    [SerializeField, Tooltip("How slow the passive hunger builds up towards the maximum value. The higher the slower"), Range(0.3f, 3f)] float PassiveHungerFactor = 1.2f; //
    public float SadnessFatigueFactor = 1.25f;
    public float SadnessHungerFactor = 1.12f;
    private float secondsToMaxFatigue = 120f;
    private float secondsToMaxHunger = 150f;
    private Dictionary<string, bool> HasSpawnedNotification = new Dictionary<string, bool>();
    private Dictionary<string, bool> HasSpawnedNotificationExtraCritical = new Dictionary<string, bool>();

    // Props
    public bool IsActive { get; private set; } = true;
    public bool isDying { get; private set; }

    // Events
    public Action<string> onStatCriticalExit;
    public Action<string, float> onStatCritical;
    public Action<string> onStatExtraCritical;
    public Action<float, float> onHealthChanged;
    public Action<DamageReason> onDeath;

    private Dictionary<string, float> lastUpdatedStatFromStart = new Dictionary<string, float>();
    private Dictionary<string, float> timeFromStatCritical = new Dictionary<string, float>();
    private float lastHealthValue;
    private GamePlayer player;
    GAgent agent;



    private void Start()
    {
        this.agent = GetComponent<GAgent>();
        player = GetComponentInParent<GamePlayer>();
        lastHealthValue = MaxHealth;
        Health = MaxHealth;
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
        if (FriendshipManager.Instance != null)
        {
            MaxSadness = FriendshipManager.Instance.MaxFriendship;
            SadnessThreshold = (int)FriendshipManager.Instance.MaxFriendship - FriendshipManager.Instance.MaxFriendship / 5f;
        }
        else
        {
            MaxSadness = 100f;
            SadnessThreshold = 80f;
        }
        ResetNotifications();

    }

    private void Update()
    {
        if (IsActive)
        {
            CalculateFatigue();
            CalculateHunger();
            CalculateSadness();

            CheckCriticalStats();
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
        HasSpawnedNotificationExtraCritical = new Dictionary<string, bool>()
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

    private void CheckCriticalStats()
    {
        foreach(KeyValuePair<string, bool> pair in HasSpawnedNotification)
        {
            if (pair.Value)
            {
                if (!timeFromStatCritical.ContainsKey(pair.Key))
                {
                    timeFromStatCritical.Add(pair.Key, 0f);
                }
                else
                {
                    timeFromStatCritical[pair.Key] += Time.deltaTime;
                    if (timeFromStatCritical[pair.Key] > 20f && damagingStatsOnExtraCritical.Contains(pair.Key))
                    {
                        if (!HasSpawnedNotificationExtraCritical[pair.Key] && NotificationManager.Instance != null)
                        {
                            NotificationManager.Instance.Warning($"Dying of {pair.Key.ToLower()}", "You have been ditching your player. Please take action");
                            HasSpawnedNotificationExtraCritical[pair.Key] = true;
                        }
                        TakeDamage(percentageDamagerPerFrame / MaxHealth, DamageReason.HUNGER);
                    }
                }
            }
        }
    }

    public void SetFatigue(float value)
    {
        string key = "Fatigue";
        this.Fatigue = Mathf.Clamp(value, 0f, MaxFatigue);
        Stats[key] = this.Fatigue;
        this.agent.beliefs.ModifyState(key, this.Fatigue);
        if (this.Fatigue >= FatigueThreshold)
        {
            this.agent.beliefs.ModifyState("IsTired", true);
            if (!HasSpawnedNotification[key])
            {
                onStatCritical?.Invoke(key, this.Fatigue);
                HasSpawnedNotification[key] = true;
            }
        }
        else
        {
            if (HasSpawnedNotification[key])
            {
                onStatCriticalExit?.Invoke(key);
                HasSpawnedNotification[key] = false;
            }
            this.agent.beliefs.RemoveState("IsTired");
            HasSpawnedNotificationExtraCritical[key] = false;
            timeFromStatCritical[key] = 0f;
        }
    }

    public void SetHunger(float value)
    {
        string key = "Hunger";
        this.Hunger = Mathf.Clamp(value, 0f, MaxHunger);
        Stats[key] = this.Hunger;
        this.agent.beliefs.ModifyState(key, this.Hunger);
        if (this.Hunger >= HungerThreshold)
        {
            this.agent.beliefs.ModifyState("IsHungry", true);
            if (!HasSpawnedNotification[key])
            {
                onStatCritical?.Invoke(key, this.Hunger);
                HasSpawnedNotification[key] = true;
            }
        }
        else
        {
            if (HasSpawnedNotification[key])
            {
                onStatCriticalExit?.Invoke(key);
                HasSpawnedNotification[key] = false;
            }
            this.agent.beliefs.RemoveState("IsHungry");
            HasSpawnedNotificationExtraCritical[key] = false;
            timeFromStatCritical[key] = 0f;
        }
    }
    public void SetSadness(float value)
    {
        if (!player.beliefs.HasState("IsSad")) { player.beliefs.AddState("IsSad", true); }
        string key = "Sadness";
        this.Sadness = Mathf.Clamp(value, 0f, MaxSadness);
        Stats[key] = this.Sadness;
        this.agent.beliefs.ModifyState(key, this.Sadness);
        if (this.Sadness >= SadnessThreshold)
        {
            this.agent.beliefs.ModifyState("IsSad", true);
            if (!HasSpawnedNotification[key])
            {
                onStatCritical?.Invoke(key, this.Sadness);
                HasSpawnedNotification[key] = true;
            }
        }
        else
        {
            if (HasSpawnedNotification[key])
            {
                onStatCriticalExit?.Invoke(key);
                HasSpawnedNotification[key] = false;
            }
            //this.agent.beliefs.RemoveState("IsSad");
            HasSpawnedNotificationExtraCritical[key] = false;
            timeFromStatCritical[key] = 0f;
        }
    }

    public void TotalEat()
    {
        SetHunger(0f);
    }

    public void TotalRest()
    {
        SetFatigue(0f);
    }

    public void PartialRest(float fatigueDecrease, float hungerIncrease)
    {
        SetFatigue(this.Fatigue - fatigueDecrease);
        SetHunger(this.Hunger + hungerIncrease);
    }

    private void OnDayPartChange(DayPart part1, DayPart part2)
    {
        if (fatigueCalculationMode == FatigueCalculationMode.TIME)
        {
            if (part2 == DayPart.EVENING)
            {
                SetFatigue(this.Fatigue + 0.75f * MaxFatigue);
            }
        }
    }

    private void CalculateFatigue()
    {
        if (fatigueCalculationMode == FatigueCalculationMode.PASSIVE)
        {
            float baseFatigue = (float)this.agent.beliefs.states["Fatigue"];
            float sadnessFactor = player.beliefs.HasState("IsSad") ? SadnessFatigueFactor : 1f;
            float newFatigue = baseFatigue + MaxFatigue / secondsToMaxFatigue * PassiveFatigueFactor * sadnessFactor * Time.deltaTime;
            this.SetFatigue(newFatigue);
        }
    }

    private void CalculateHunger()
    {
        float baseHunger = (float)this.agent.beliefs.states["Hunger"];
        float sadnessFactor = player.beliefs.HasState("IsSad") ? SadnessHungerFactor : 1f;
        float newHunger = baseHunger + MaxHunger / secondsToMaxHunger * PassiveHungerFactor * sadnessFactor * Time.deltaTime;
        newHunger = Mathf.Clamp(newHunger, 0f, MaxHunger);
        this.SetHunger(newHunger);
    }

    private void CalculateSadness()
    {
        if (FriendshipManager.Instance != null)
        {
            float sadness = Mathf.Clamp(
                FriendshipManager.Instance.MaxFriendship - FriendshipManager.Instance.AverageFriendship,
                0f, FriendshipManager.Instance.MaxFriendship
            );
            this.SetSadness(sadness);

        }
    }


    public void TakeDamage(float value, DamageReason reason)
    {
        if (isDying)
        {
            return;
        }
        value = Mathf.Abs(value);
        Health = Mathf.Max(Health - value, 0f);
        onHealthChanged?.Invoke(lastHealthValue, Health);
        if (Health <= 0f && player.playerState != GamePlayer.PlayerState.DEAD)
        {
            isDying = true;
            Die(reason);
            player.SetPlayerState(GamePlayer.PlayerState.DEAD);
        }
    }
    public void GainHealth(float value)
    {
        value = Mathf.Abs(value);
        Health = Mathf.Min(MaxHealth, Health + value);
        onHealthChanged?.Invoke(lastHealthValue, Health);
        lastHealthValue = Health;
    }

    public void Activate(bool isActive)
    {
        IsActive = isActive;
    }

    public void Die(DamageReason reason)
    {
        onDeath?.Invoke(reason);
        Activate(false);
        TimeManager.Instance.Pause();
    }
}
