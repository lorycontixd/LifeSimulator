using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatPatient : GAction
{
    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("TreatedPatient", 1);
        Cubicle c = target.GetComponent<Cubicle>();
        if (c == null)
        {
            return false;
        }
        HospitalManager.Instance.AddCubicle(c);
        inventory.RemoveItem(target);
        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", 1);
        Nurse nurse = this.GetComponent<Nurse>();
        if (nurse == null)
        {
            Debug.LogError($"Expected Nurse object for TreatPatient action, but was not found", this);
        }
        nurse.GetTired(nurse.MaxFatigue / 4);
        this.GetComponent<GAgent>().ResetNavmeshSpeed();
        if (HospitalManager.Instance.patientsCount <= 0)
        {
            this.beliefs.ModifyState("CanRest", true);
        }
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
