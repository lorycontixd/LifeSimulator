using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requirement for a transition
public delegate bool FSMCondition();

// Function to perform action
public delegate void FSMAction();

public class FSMTransition
{
    // Condition to evaluate for transition to fire
    public FSMCondition condition;

    // List of actions to fire when transition fires
    private List<FSMAction> actions = new List<FSMAction>();

    public FSMTransition(FSMCondition condition, List<FSMAction> actions = null)
    {
        this.condition = condition;
        this.actions = actions;
    }

    public void Fire()
    {
        if (actions != null)
        {
            foreach(FSMAction action in actions)
            {
                action();
            }
        }
    }
}
