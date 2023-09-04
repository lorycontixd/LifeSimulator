using GOAP;
using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    public override bool PostPerform()
    {
        NewPlayerStats stats = this.GetComponentInParent<NewPlayerStats>();
        if (stats != null)
        {
            stats.TotalEat();
            beliefs.ModifyState("LastAction", actionName);
            return true;
        }
        return false;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
