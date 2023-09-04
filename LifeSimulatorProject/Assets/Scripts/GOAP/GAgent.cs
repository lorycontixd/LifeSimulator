using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GOAP;
using System;
using UnityEngine.AI;
using Lore.Game.Characters;

public class Goal {

    public Dictionary<string, int> sGoals;
    public bool remove;


    public Goal(string s, int i, bool r) {

        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}

public class GAgent : Character {

    public List<GAction> actions = new List<GAction>();
    public Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
    public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();
    public bool CanSearchPlan { get; set; }
    public string CurrentGoalStr { get; private set; } = "";
    public bool IsNavmeshOverriden { get; private set; } = false;
    public bool invoked { get; private set; } = false;

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    Goal currentGoal;
    protected NavMeshAgent navmeshAgent;
    private bool isAgentPaused;
    private float defaultAgentSpeed;

    public float actionCompletionDistance = 2.2f;
    [Range(0f,0.9f)] public float DurationModifierPercentageMax = 0.4f;
    public float DurationModifierPerc { get; protected set; } = 0f;
    public bool IsSpeedReduced { get; private set; }

    public Action<GAction, float> onActionComplete;
    public Action<Vector3> onDestinationSet;
    public Action<string> onPlanFound;

    [SerializeField] private bool _planDebugMode;


    protected virtual void Start() {
        planner = null;
        CanSearchPlan = true;
        GAction[] acts = this.GetComponentsInChildren<GAction>();
        acts = acts.Where(a => a.enabled).ToArray();
        
        if (navmeshAgent == null)
        {
            navmeshAgent = GetComponentInParent<NavMeshAgent>();
        }
        foreach (GAction a in acts) {

            actions.Add(a);
        }
        defaultAgentSpeed = navmeshAgent.speed;
    }

    public void ForceActionComplete()
    {
        CompleteAction();
        planner = null;
        currentGoal = null;
        CurrentGoalStr = string.Empty;
    }

    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
        currentAction = null;
    }

    void LateUpdate()
    {
        if (_planDebugMode)
        Debug.Log($"[GAgent->LU] currentaction: {currentAction}, isnull: {currentAction == null}");
        if (currentAction != null && _planDebugMode)
        {
            Debug.Log($"[GAgent->LU] current action not null ==> isrunning: {currentAction.running}");
        }
        if (currentAction != null && currentAction.running ) {
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            //Debug.Log($"[GAgent] Dist to target: {distanceToTarget} / {actionCompletionDistance}");
            if (currentAction.agent.hasPath && distanceToTarget < actionCompletionDistance)//currentAction.agent.remainingDistance < 0.5f)
            {
                if (_planDebugMode) Debug.Log($"Current action not null, reached dest");
                if (!invoked) {
                    if (_planDebugMode) Debug.Log($"Current action not null, reached dest, not invoked => Calling complete action!");
                    currentAction.OnArrival();
                    float actualDuration = 0;
                    if (currentAction.duration > 0)
                    {
                        actualDuration = currentAction.duration *  (1f - DurationModifierPerc);
                    }
                    else
                    {
                        actualDuration = currentAction.duration;
                    }
                    Invoke("CompleteAction", actualDuration);
                    invoked = true;
                    onActionComplete?.Invoke(currentAction, actualDuration);
                }
            }
            return;
        }

        if ( (planner == null || actionQueue == null) && CanSearchPlan)
        {
            if (_planDebugMode) Debug.Log($"No action queue, planning new => p:{planner==null}, aq:{actionQueue==null}");
            planner = new GPlanner(debugMode: false);
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            foreach (KeyValuePair<Goal, int> sg in sortedGoals) {
                actionQueue = planner.Plan(actions, sg.Key.sGoals, beliefs);

                if (actionQueue != null) {

                    currentGoal = sg.Key;
                    CurrentGoalStr = sg.Key.sGoals.Last().Key;
                    string plan = BuildPlanString(actionQueue);
                    if (_planDebugMode) Debug.Log($"Found plan ==> {plan}");
                    onPlanFound?.Invoke(plan);
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (_planDebugMode)
                Debug.Log($"Action q count == 0: {actionQueue.Count == 0}, isnull: {actionQueue == null}");
            // FInished all actions for a goal
            if (currentGoal.remove) {

                goals.Remove(currentGoal);
            }
            CurrentGoalStr = string.Empty;
            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0)
        {
            if (_planDebugMode)
                Debug.Log($"ActionQ not null, action q count > 0 => {actionQueue.Count}");
            // Finished one action for goal queue, but goal queue is not empty
            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform()) {

                if (currentAction.target == null && currentAction.targetTag != "") {

                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                currentAction.running = true;
                if (currentAction.target != null && !IsNavmeshOverriden)
                {
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                    //onDestinationSet?.Invoke(currentAction.target.transform.position);
                }
            } else {

                actionQueue = null;
            }
        }
    }

    private string BuildPlanString(Queue<GAction> queue)
    {
        List<GAction> list = queue.ToList();
        string result = "";
        for (int i=0; i<list.Count; i++)
        {
            result += list[i].actionName + ", ";
        }
        result = result.Remove(result.Length - 2, 2);
        return result;
    }

    public virtual void PauseAgent()
    {
        navmeshAgent.isStopped = true;
    }
    public virtual void ResumeAgent()
    {
        navmeshAgent.isStopped = false;
    }

    public bool AddGoal(string key, int priority, bool removeAtCompletion = false)
    {
        Goal g = new Goal(key, 1, removeAtCompletion);
        goals.Add(g, priority);
        return true;
    }

    public float GetNavmeshSpeed()
    {
        return navmeshAgent.speed;
    }
    public void SetNavmeshSpeed(float speed)
    {
        if (speed < 0)
        {
            return;
        }
        navmeshAgent.speed = speed;
    }
    public void ReduceNavmeshSpeed(float factor)
    {
        if (factor <= 0)
        {
            return;
        }
        navmeshAgent.speed = navmeshAgent.speed / factor;
        IsSpeedReduced = true;
    }
    public void ResetNavmeshSpeed()
    {
        navmeshAgent.speed = defaultAgentSpeed;
        IsSpeedReduced = false;
    }

    public void OverrideNavmesh(bool _override)
    {
        this.IsNavmeshOverriden = _override;
    }
}
