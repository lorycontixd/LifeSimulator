using GOAP;
using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatPatient : GAction
{
    public override bool PostPerform()
    {
        Debug.Log($"[Nurse->Treat] starting post perform");
        GWorld.Instance.GetWorld().ModifyState("TreatedPatient", 1);
        Cubicle c = target.GetComponent<Cubicle>();
        if (c == null)
        {
            return false;
        }
        HospitalManager.Instance.AddCubicle(c);
        inventory.RemoveItem(target);
        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", 1);

        Debug.Log($"[Nurse->Treat] curing all diseases");
        if (SicknessManager.Instance != null)
            SicknessManager.Instance.CureAll();

        Nurse nurse = this.GetComponent<Nurse>();
        if (nurse == null)
        {
            Debug.LogError($"Expected Nurse object for TreatPatient action, but was not found", this);
            return false;
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
        Debug.Log($"[Nurse->Treat] starting preperform");
        target = inventory.FindItemWithTag("Cubicle");
        Debug.Log($"[Nurse->Treat] setting cubicle target");
        if (target == null)
        {
            return false;
        }
        return true;
    }
}