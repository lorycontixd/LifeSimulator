using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDog : GAction
{
    private Dog dog;
    public override bool PostPerform()
    {
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        Transform park = BuildingManager.Instance.GetRandomPark();
        if (park == null) { return false; }
        Dog dog = FindFirstObjectByType<Dog>();
        if (dog == null) { return false; }
        dog.StartDogWalk();
        target = park.gameObject;
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if ((string)conditions["LastAction"] == actionName) { return false; }
        return base.IsAchievableGiven(conditions);
    }
}
