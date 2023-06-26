using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeRest : GAction
{
    [SerializeField] private float HungerCost = 1.5f;
    [SerializeField] private float FatigueValue = 2f;

    public override bool PostPerform()
    {
        this.beliefs.ModifyState("Hunger", (float)this.beliefs.states["Hunger"] + HungerCost);
        this.beliefs.ModifyState("Fatigue", (float)this.beliefs.states["Fatigue"] - FatigueValue);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
