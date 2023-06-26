using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHome : GAction
{
    public override bool PostPerform()
    {
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
