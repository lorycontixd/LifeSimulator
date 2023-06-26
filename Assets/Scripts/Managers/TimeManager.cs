using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using System;
using UnityEngine.UIElements;
using Lore.Game.Managers;
using Lore.Game.Characters;

public enum DayPart
{
    MORNING,
    AFTERNOON,
    EVENING,
    NIGHT
}

public class TimeManager : BaseManager
{
    #region Singleton
    private static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

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

    [SerializeField, Range(1f, 4f)] private int simulationSpeedUpFactor = 1;


    [Header("Game Settings")]
    public bool IsActive = true;
    [SerializeField, Range(0.5f, 6)] private int dayDurationMinutes = 4;

    public DayPart CurrentDayPart { get; private set; } = DayPart.MORNING;
    public int DaysPassed { get { return _daysPassed; } }
    public float DayPercentage { get { return _partOfDay; } }
    public int CurrentDay { get; private set; } = 0;
    public int CurrentMonth { get; private set; } = 0;
    private DayPart lastDayPart = DayPart.MORNING;
    private int lastDaysPassed;
    private float morningPerc = 0.3f;
    private float afternoonPerc = 0.2f;
    private float eveningPerc = 0.2f;
    private float nightPerc = 0.3f;
    private int _daysPassed = 0;
    private int _monthDay = 0;
    private float _partOfDay = 0f;
    private Dictionary<DayPart, string> dayPartsStates = new Dictionary<DayPart, string>();
    private Dictionary<DayPart, float> dayPartDurations = new Dictionary<DayPart, float>();

    public Action<DayPart, DayPart> onDayPartChange; // Params = (old day part, new day part)
    public Action<int> onNewDay;
    public Action<int> onNewMonth;


    public override void Start()
    {
        Time.timeScale = simulationSpeedUpFactor;
        GWorld.Instance.GetWorld().ModifyState("DayPart", CurrentDayPart);
        GWorld.Instance.GetWorld().AddState($"Is{CurrentDayPart.ToString().ToLower().Capitalize()}", true);

        foreach (var value in Enum.GetValues(typeof(DayPart))){
            DayPart v = (DayPart)value;
            dayPartsStates.Add(v, $"Is{v.ToString().ToLower().Capitalize()}");
        }
        dayPartDurations = new Dictionary<DayPart, float>
        {
            { DayPart.MORNING, morningPerc * dayDurationMinutes },
            { DayPart.AFTERNOON, afternoonPerc * dayDurationMinutes },
            { DayPart.EVENING, eveningPerc * dayDurationMinutes },
            { DayPart.NIGHT, nightPerc * dayDurationMinutes }
        };

        base.Start();
    }

    void Update()
    {
        if (!IsActive) return;
        CalculateDayPart();
    }

    public float GetDayPartDuration(DayPart dayPart)
    {
        return dayPartDurations[dayPart];
    }

    private void CalculateDayPart()
    {
        float timeSinceStart = Time.realtimeSinceStartup;
        _daysPassed = (int)(timeSinceStart / 60f) / dayDurationMinutes;
        _partOfDay = timeSinceStart / (dayDurationMinutes * 60f) % 1;
        if (_partOfDay <= morningPerc)
        {
            CurrentDayPart = DayPart.MORNING;
        }
        else if(_partOfDay > morningPerc && _partOfDay <= morningPerc + afternoonPerc)
        {
            CurrentDayPart = DayPart.AFTERNOON;
        }
        else if(_partOfDay > afternoonPerc && _partOfDay <= morningPerc + afternoonPerc + eveningPerc)
        {
            CurrentDayPart = DayPart.EVENING;
        }
        else
        {
            CurrentDayPart = DayPart.NIGHT;
        }
        if (CurrentDayPart != lastDayPart)
        {
            onDayPartChange?.Invoke(lastDayPart, CurrentDayPart);
            GWorld.Instance.GetWorld().ModifyState("DayPart", CurrentDayPart);
            foreach(KeyValuePair<DayPart, string> kvp in dayPartsStates)
            {
                if (kvp.Key == CurrentDayPart)
                {
                    GWorld.Instance.GetWorld().AddState(kvp.Value, true);
                }
                else
                {
                    GWorld.Instance.GetWorld().RemoveState(kvp.Value);
                }
            }
        }
        if (lastDaysPassed != _daysPassed)
        {
            onNewDay?.Invoke(_daysPassed);
            CurrentDay++;
            if (_daysPassed % 31 == 0)
            {
                CurrentDay = 0;
                CurrentMonth++;
                onNewMonth?.Invoke(CurrentMonth);
            }
        }
        lastDayPart = CurrentDayPart;
        lastDaysPassed = _daysPassed;
    }

    public void Pause()
    {
        GamePlayer player = FindFirstObjectByType<GamePlayer>();
        if (player != null)
        {
            player.PauseAgent();
        }
        IsActive = false;
    }
    public void Resume()
    {
        GamePlayer player = FindFirstObjectByType<GamePlayer>();
        if (player != null)
        {
            player.ResumeAgent();
        }
        IsActive = true;
    }
}
