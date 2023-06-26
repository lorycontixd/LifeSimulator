using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : GAction
{
    [SerializeField] private float HungerCost = 3f;

    public override bool PostPerform()
    {
        this.beliefs.ModifyState("Hunger", (float)this.beliefs.states["Hunger"] + HungerCost);
        this.beliefs.ModifyState("Fatigue", 0f);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievable()
    {
        return TimeManager.Instance.CurrentDayPart == DayPart.NIGHT;
    }

}
