using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using Lore.Game.Characters;

public class GoHome : GAction
{
    public override bool PostPerform()
    {
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if (!conditions.ContainsKey("IsHome")) { return  false; }
        return !(bool)conditions["IsHome"] && base.IsAchievableGiven(conditions);
    }
}
