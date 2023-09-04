using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveHospital : GAction
{
    public override bool PostPerform()
    {
        this.GetComponentInParent<GAgent>().ResetNavmeshSpeed();
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }

    public override bool IsAchievable()
    {
        bool isAtHospital = beliefs.HasState("IsAtHospital");
        bool _base = base.IsAchievable();
        return isAtHospital && _base;
    }
}
