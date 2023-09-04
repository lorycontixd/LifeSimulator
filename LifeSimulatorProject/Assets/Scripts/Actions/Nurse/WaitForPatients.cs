using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPatients : GAction
{

    public override bool PostPerform()
    {
        Nurse nurse = this.GetComponent<Nurse>();
        if (nurse == null)
        {
            Debug.LogError($"Expected Nurse object for WaitForPatients action, but was not found", this);
        }
        nurse.Rest();
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
