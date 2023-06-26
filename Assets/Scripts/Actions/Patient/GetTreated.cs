using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTreated : GAction
{
    public override bool PostPerform()
    {
        //GWorld.Instance.GetWorld().ModifyState("TreatedPatient", 1);
        this.inventory.RemoveItem(target);
        beliefs.ModifyState("IsCured", 1);
        return true;
    }

    public override bool PrePerform()
    {
        target = inventory.FindItemWithTag("Cubicle");
        if (target == null)
        {
            return false;
        }

        return true;
    }
}
