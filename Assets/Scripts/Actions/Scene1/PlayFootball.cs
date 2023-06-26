using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFootball : GAction
{
    public float FatigueBoost = 2f;
    public float HappinessNewValue = 0f;

    public override bool PostPerform()
    {
        this.beliefs.ModifyState("Sadness", HappinessNewValue);
        this.beliefs.ModifyState("Fatigue", (float)this.beliefs.states["Fatigue"] + FatigueBoost);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

}
