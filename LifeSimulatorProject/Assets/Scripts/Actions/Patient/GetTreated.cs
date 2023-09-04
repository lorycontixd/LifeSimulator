using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTreated : GAction
{
    public override bool PostPerform()
    {
        Debug.Log($"[Agent->GetTreated] start postperform");
        //GWorld.Instance.GetWorld().ModifyState("TreatedPatient", 1);
        this.inventory.RemoveItem(target);
        Debug.Log($"[Agent->GetTreated] setting states");
        this.beliefs.ModifyState("IsCured", 1);
        this.beliefs.RemoveState("NeedsCures");
        beliefs.ModifyState("LastAction", actionName);
        if (beliefs.HasState("NursePickedUp"))
        {
            beliefs.RemoveState("NursePickedUp");
        }
        Debug.Log($"[Agent->GetTreated] end postperform");
        return true;
    }

    public override bool PrePerform()
    {
        Debug.Log($"[Agent->GetTreated] starting preperform");
        target = inventory.FindItemWithTag("Cubicle");
        Debug.Log($"Fetching free cubicle");
        if (target == null)
        {
            return false;
        }

        Debug.Log($"[Agent->GetTreated] end preperform");
        return true;
    }
}
