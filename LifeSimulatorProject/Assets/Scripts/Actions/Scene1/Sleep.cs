using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : GAction
{
    [SerializeField] private float HungerCost = 3f;

    public override bool PostPerform()
    {
        NewPlayerStats stats = GetComponentInParent<NewPlayerStats>();
        stats.TotalRest();
        stats.SetHunger((float)this.beliefs.states["Hunger"] + HungerCost);
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievable()
    {
        return TimeManager.Instance.CurrentDayPart == DayPart.NIGHT && base.IsAchievable();
    }

}
