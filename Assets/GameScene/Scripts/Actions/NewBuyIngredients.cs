using Lore.Game.Buildings;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuyIngredients : GAction
{
    public override bool PostPerform()
    {
        return true;
    }

    public override bool PrePerform()
    {
        if (target == null && targetTag == string.Empty)
        {
            Lore.Game.Buildings.Building b = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.SUPERMARKET);
            if (b != null)
            {
                target = b.gameObject;
            }
            else
            {
                Debug.LogWarning($"[BuyIngredients] No supermarket found in preperform");
            }
        }
        return true;
    }

    public override bool IsAchievable()
    {
        if (BuildingManager.Instance == null)
        {
            return false;
        }
        return (BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.SUPERMARKET));
    }
}
