using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : GAction
{
    public override bool PostPerform()
    {
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
