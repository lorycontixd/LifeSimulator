using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private FSMState current;
    private List<FSMState> states = new List<FSMState>();
    private List<FSMTransition> transitions = new List<FSMTransition>();

    public FSM(FSMState state) {
        this.current = state;
        this.current.Enter();
    }

    /// <summary>
    /// Examine all transitions leading out from the current state:
    /// (1) Execute actions on exit from current state
    /// (2) Execute actions on firing transition
    /// (3) Get target state and set it as current state
    /// (4) Execute actions on new state enter
    /// If no action is activated
    /// (5) Execute actions on staying in the same state
    /// </summary>
    public void Update()
    {
        FSMTransition transition = current.VerifyTransition();
        if (transition != null)
        {
            // At this point, any transition from the current state met its condition
            current.Exit(); // (1)
            transition.Fire(); // (2)
            current = current.NextState(transition); // (3)
            current.Enter(); // (4)
        }
        else
        {
            current.Stay(); // (5)
        }
    }

    public void RegisterState(FSMState state)
    {
        states.Add(state);
    }
    public void RegisterStates(FSMState[] s)
    {
        foreach (FSMState state in s)
        {
            states.Add(state);
        }
    }
}
