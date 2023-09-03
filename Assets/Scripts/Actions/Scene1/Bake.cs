using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bake : GAction
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

    public override bool IsAchievable()
    {
        bool isTired = beliefs.HasState("IsTired");
        bool hasDog = beliefs.HasState("DogFollowing");
        return !isTired && !hasDog && base.IsAchievable();
    }
}
