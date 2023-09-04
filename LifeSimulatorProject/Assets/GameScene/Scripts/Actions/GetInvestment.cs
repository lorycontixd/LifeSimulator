using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInvestment : GAction
{
    public override bool PostPerform()
    {
        InvestManager.Instance.AcceptPendingInvestments();
        return true;
    }

    public override bool PrePerform()
    {
        if (BuildingManager.Instance == null) return false;
        Lore.Game.Buildings.Building bank = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.BANK);
        if (bank == null) return false;
        target = bank.gameObject;
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if (!conditions.ContainsKey("PendingInvestment")) { return false; }
        return base.IsAchievableGiven(conditions);
    }
}
