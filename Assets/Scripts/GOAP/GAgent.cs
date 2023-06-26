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
    bool invoked = false;

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    Goal currentGoal;
    protected NavMeshAgent navmeshAgent;
    private bool isAgentPaused;
    private float defaultAgentSpeed;

    [SerializeField] private float actionCompletionDistance = 2.2f;
    public Action<GAction> onActionComplete;
    public Action<Vector3> onDestinationSet;


    protected virtual void Start() {

        GAction[] acts = this.GetComponentsInChildren<GAction>();
        if (navmeshAgent == null)
        {
            navmeshAgent = GetComponentInParent<NavMeshAgent>();
        }
        foreach (GAction a in acts) {

            actions.Add(a);
        }
        defaultAgentSpeed = navmeshAgent.speed;
    }

    void CompleteAction() {

        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
        currentAction = null;
    }

    void LateUpdate() {

        if (currentAction != null && currentAction.running) {

            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            if (currentAction.agent.hasPath && distanceToTarget < actionCompletionDistance)//currentAction.agent.remainingDistance < 0.5f)
                {
                if (!invoked) {

                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                    onActionComplete?.Invoke(currentAction);
                }
            }
            return;
        }

        if (planner == null || actionQueue == null) {

            planner = new GPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<Goal, int> sg in sortedGoals) {

                actionQueue = planner.Plan(actions, sg.Key.sGoals, beliefs);

                if (actionQueue != null) {

                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0) {

            if (currentGoal.remove) {

                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0) {

            currentAction = actionQueue.Dequeue();

            if (currentAction.PrePerform()) {

                if (currentAction.target == null && currentAction.targetTag != "") {

                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                if (currentAction.target != null) {

                    currentAction.running = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                    onDestinationSet?.Invoke(currentAction.target.transform.position);
                }
            } else {

                actionQueue = null;
            }
        }
    }

    public void PauseAgent()
    {
        navmeshAgent.isStopped = true;
    }
    public void ResumeAgent()
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
    public void ResetNavmeshSpeed()
    {
        navmeshAgent.speed = defaultAgentSpeed;
    }
}
