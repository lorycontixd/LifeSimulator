using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHome : GAction
{
    private Dog dog;

    public override bool PostPerform()
    {
        if ( beliefs.GetStates().ContainsKey("DogFollowing"))
        {
            beliefs.RemoveState("DogFollowing");
        }
        dog.EndDogWalk();
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        dog = FindFirstObjectByType<Dog>();
        if (dog == null) return false;
        dog.ReturningHome();
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if ((string)conditions["LastAction"] == actionName) { return false; }
        return base.IsAchievableGiven(conditions);
    }
}
