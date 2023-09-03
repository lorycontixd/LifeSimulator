using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour {

    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target;
    public string targetTag;
    public float duration = 0.0f;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;
    public Dictionary<string, object> preconditions;
    public Dictionary<string, object> effects;
    public WorldStates agentBeliefs;
    public GInventory inventory;
    public WorldStates beliefs;
    public bool running = false;

    public GAction() {

        preconditions = new Dictionary<string, object>();
        effects = new Dictionary<string, object>();
    }

    private void Awake() {

        agent = this.gameObject.GetComponentInParent<NavMeshAgent>();

        if (preConditions != null) {

            foreach (WorldState w in preConditions) {

                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null) {

            foreach (WorldState w in afterEffects) {

                effects.Add(w.key, w.value);
            }
        }

        inventory = this.GetComponentInParent<GAgent>().inventory;
        beliefs = this.GetComponentInParent<GAgent>().beliefs;
    }

    public virtual bool IsAchievable() {

        return true;
    }

    public virtual bool IsAchievableGiven(Dictionary<string, object> conditions) {

        foreach (KeyValuePair<string, object> p in preconditions) {

            if (!conditions.ContainsKey(p.Key)) return false;
        }

        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();

    /// <summary>
    /// This function is executed on arrival to the destination,
    /// but before the duration timer.
    /// </summary>
    /// <returns>bool: If the arrival function was successful</returns>
    public virtual bool OnArrival()
    {
        return true;
    }
}