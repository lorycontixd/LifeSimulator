using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMState {
    // List of actions to perform based on transition
    public List<FSMAction> enterActions = new List<FSMAction>();
    public List<FSMAction> stayActions = new List<FSMAction>();
    public List<FSMAction> exitActions = new List<FSMAction>();

    // List of transitions and states they lead to
    private Dictionary<FSMTransition, FSMState> links;

    public FSMState()
    {
        links = new Dictionary<FSMTransition, FSMState>();
    }
    public void AddTransition(FSMTransition transition, FSMState state)
    {
        links[transition ] = state;
    }
    public FSMTransition VerifyTransition()
    {
        foreach(FSMTransition t in links.Keys)
        {
            if (t.condition())
            {
                return t;
            }
        }
        return null;
    }
    public FSMState NextState(FSMTransition transition)
    {
        return links[transition ];
    }

    public void Enter() { foreach (FSMAction a in enterActions) a(); }
    public void Stay() { foreach (FSMAction a in stayActions) a(); }
    public void Exit() { foreach (FSMAction a in exitActions) a(); }
}
