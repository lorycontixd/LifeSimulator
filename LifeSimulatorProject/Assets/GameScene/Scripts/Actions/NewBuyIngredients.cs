using Lore.Game.Buildings;
using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuyIngredients : GAction
{
    [SerializeField] private float MoneyCost;

    public override bool PostPerform()
    {
        beliefs.ModifyState("LastAction", actionName);
        MoneyManager.Instance.SpendMoney(MoneyCost);
        return true;
    }

    public override bool PrePerform()
    {
        if (!MoneyManager.Instance.CanAfford(MoneyCost)){
            if (!beliefs.HasState("NeedsMoney"))
                beliefs.AddState("NeedsMoney", true);
            GamePlayer player = GetComponentInParent<GamePlayer>();
            Debug.Log($"Cant afford buy ingredients => setting steal money goal");
            player.AddGoal("StealMoney", 5, true);
            return false;
        }
        if (target == null && targetTag == string.Empty)
        {
            Lore.Game.Buildings.Building b = null;
            if (BuildingManager.Instance.IsBuildingConstructed(BuildingData.BuildingType.SUPERMARKET))
            {
                b = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.SUPERMARKET);
            }
            else
            {
                b = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.MINIMARKET);
            }
            if (b != null)
            {
                target = b.gameObject;
            }
            else
            {
                Debug.LogWarning($"[BuyIngredients] No supermarket found in preperform");
                return false;
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
        bool building = (BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.SUPERMARKET) || BuildingManager.Instance.IsBuildingConstructed(BuildingData.BuildingType.MINIMARKET));
        bool hasMoney = MoneyManager.Instance.CanAfford(MoneyCost);
        return building && base.IsAchievable();
    }
}
