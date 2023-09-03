using GOAP;
using Lore.Game.Characters;
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
        NewPlayerStats stats = GetComponentInParent<GamePlayer>().GetComponent<NewPlayerStats>();
        stats.PartialRest(FatigueValue, HungerCost);
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if (!conditions.ContainsKey("Fatigue")) { return false; }
        if (!conditions.ContainsKey("IsHome")) { return false; }
        if ((string)conditions["LastAction"] == actionName) { return false; }
        //return (bool)conditions["IsHome"];
        return base.IsAchievableGiven(conditions);
    }
}
