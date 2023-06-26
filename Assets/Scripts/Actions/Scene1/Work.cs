using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work : GAction
{
    public override bool PostPerform()
    {
        this.beliefs.AddState("HasWorkedToday", true);
        
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievable()
    {
        return !this.beliefs.states.ContainsKey("HasWorkedToday");
    }
}
