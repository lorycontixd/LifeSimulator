using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;

public class Rest : GAction
{
    public override bool PostPerform()
    {
        Nurse nurse = this.GetComponent<Nurse>();
        if (nurse == null)
        {
            Debug.LogError($"Expected Nurse object for Rest action, but was not found", this);
        }
        nurse.Rest();
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
