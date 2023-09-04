using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : GAction
{
    private Bot bot;

    public override bool PostPerform()
    {
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
