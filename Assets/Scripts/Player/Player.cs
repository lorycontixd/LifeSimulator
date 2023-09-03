using GOAP;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class Player : GAgent
{
    private PlayerStats stats;
    private Bot bot;

    public Personality Personality;
    public float Money { get; private set; } = 100f;
    public Dictionary<NPC, float> friendships = new Dictionary<NPC, float>(); // Key = npc, value = value of friendship
    [SerializeField] private Transform testTarget = null;
    [SerializeField] private bool useBot = false;

    [SerializeField, Tooltip("How often the goal weights are updated. If set to 0, the rate is the same as the internal update rate")] private float setGoalsDelay = 2f; // If delay == 0, put in update
    [SerializeField, Range(0f, 1.5f), Tooltip("How often to scan for nearby npcs. If zero, scanning is put in update")] private float npcScanRateSec = 0.4f;
    [SerializeField, Range(2f, 15f), Tooltip("NPC scan range")] private float npcScanRange = 5f;
    [SerializeField, Range(3f, 10f)] private float npcTalkCooldown = 6f;

    public NPC nearestNPC = null;// make private after debug
    public NPC talkingToNPC = null; // make private after debug
    private float npcTalkTimestamp;


    protected override void Start()
    {
        base.Start();
        stats = new PlayerStats();
        this.bot = GetComponent<Bot>();
        if (bot == null)
        {
            if (useBot)
            {
                Debug.LogWarning($"No Bot script was found on player. Adding a new Bot component");
                bot = gameObject.AddComponent<Bot>();
            }
        }

        npcTalkTimestamp = 0f;

        onActionComplete += OnActionComplete;
        //onDestinationSet += OnDestinationSet;
        TimeManager.Instance.onNewMonth += OnNewMonth;
        TimeManager.Instance.onNewDay += OnNewDay;

        if (testTarget != null)
        {
            this.bot.SetBotAction(Bot.BotAction.SEEK, testTarget.gameObject);
        }
        if (setGoalsDelay > 0)
        {
            StartCoroutine(SetGoalsCo());
        }
        if (npcScanRateSec > 0f)
        {
            StartCoroutine(ScanNearestNpcCo());
        }
        this.bot.SetBotAction(Bot.BotAction.WANDER);
        Debug.Log($"[PLayer] Setting bot action to wander");
    }
    private void Update()
    {
        if (setGoalsDelay == 0)
        {
            SetGoals();
        }
        if (npcScanRateSec <= 0f)
        {
            nearestNPC = GetNearestNPC();
        }
        if (npcTalkTimestamp > 0f)
        {
            npcTalkTimestamp -= Time.deltaTime;
        }
        
    }

    // Event listeners
    public void OnActionComplete(GAction action, float actualDuration)
    {
        if (currentAction == null)
        {
            bot.SetBotAction(Bot.BotAction.WANDER);
        }
    }
    public void OnDestinationSet(Vector3 position)
    {
        Debug.Log($"[PLayer] Destination set {position}, setting bot action to none");
        bot.SetBotAction(Bot.BotAction.NONE);
    }
    private void OnNewMonth(int obj)
    {
        if (MoneyManager.Instance == null)
        {
            return;
        }
        Money += MoneyManager.Instance.monthlySalary;
    }
    private void OnNewDay(int CurrentDay)
    {
        this.beliefs.RemoveState("HasWorkedToday");
    }

    //

    private IEnumerator SetGoalsCo()
    {
        while (true)
        {
            SetGoals();
            yield return new WaitForSeconds(setGoalsDelay);
        }
    }
    private void SetGoals()
    {
        goals.Clear();

        Goal s1 = new Goal("TotalEat", 1, false);
        goals.Add(s1, (int)stats.Hunger);

        Goal s2 = new Goal("DoSports", 1, false);
        goals.Add(s2, (int)stats.Sadness);

        Goal s3 = new Goal("GetTreated", 1, false);
        goals.Add(s3, 1);

        Goal s4 = new Goal("TotalRest", 1, false);
        goals.Add(s4, (int)stats.Fatigue);

        Goal s5 = new Goal("Sleep", 1, false);
        goals.Add(s5, (int)stats.Fatigue);

        Goal s6 = new Goal("Wander", 1, false);
        goals.Add(s6, 1);

        Goal s7 = new Goal("GWork", 5, false);
        goals.Add(s7, 5);

        Goal s8 = new Goal("Talk", 1, false);
        goals.Add(s8, 1);
    }

    #region Talking
    public (bool, string) CanTalk()
    {
        if (nearestNPC == null)
        {
            return (false, "No NPC nearby");
        }
        if (talkingToNPC != null)
        {
            return (false, "Player already talking");
        }
        if (npcTalkTimestamp > 0)
        {
            return (false, "Talking is still on cooldown");
        }
        return (true, string.Empty);
        //return nearestNPC != null && talkingToNPC == null && npcTalkTimestamp <= 0f;
    }
    public void StartTalk()
    {
        talkingToNPC = nearestNPC;
        talkingToNPC?.StartTalk(this);
        transform.LookAt(talkingToNPC.transform);
        Debug.Log($"[Player] Starting talk with {talkingToNPC.ID}");
    }
    public void StopTalk()
    {
        Debug.Log($"[Player] Stopping talk with {talkingToNPC.ID}");
        talkingToNPC?.StopTalk(this);
        talkingToNPC = null;
        npcTalkTimestamp = npcTalkCooldown;
    }
    #endregion
    private NPC GetNearestNPC()
    {
        NPC npc = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, npcScanRange);
        if (colliders.Length > 0)
        {
            Collider c = colliders.FirstOrDefault(c => c.gameObject.GetComponent<NPC>() != null);
            if (c != null)
            {
                npc = c.gameObject.GetComponent<NPC>();
            }
        }
        return npc;
    }

    private IEnumerator ScanNearestNpcCo()
    {
        while (true)
        {
            nearestNPC = GetNearestNPC();
            yield return new WaitForSeconds(npcScanRateSec);
        }
    }
    private IEnumerator SetStatesDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        this.beliefs.AddState("IsHungry", 1);
    }
    private IEnumerator StopTalkingToNPC(float delay)
    {
        yield return new WaitForSeconds(delay);
        talkingToNPC.StopTalk(this);
        talkingToNPC = null;
        npcTalkTimestamp = npcTalkCooldown;
        
    }





    /// <summary>
    /// Calculate fatigue
    /// </summary>
}
