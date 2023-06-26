using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayHospital : GAction
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
}
