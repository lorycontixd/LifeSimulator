using GOAP;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoSports : GAction
{
    public override bool PostPerform()
    {
        beliefs.ModifyState("LastAction", actionName);
        return true;
    }

    public override bool PrePerform()
    {
        target = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.SPORTS_CENTER).gameObject;
        return true;
    }

    public override bool IsAchievable()
    {
        if (!BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.SPORTS_CENTER)) { return false; }
        if (GWorld.Instance.GetWorld().HasState("DogWantsWalk")) { return false; }
        return base.IsAchievable();
    }
}
