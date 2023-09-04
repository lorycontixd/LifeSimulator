using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatRestaurant : GAction
{
    public float MoneyCost;


    public override bool PostPerform()
    {
        MoneyManager.Instance.SpendMoney(MoneyCost);
        NewPlayerStats p = GetComponentInParent<NewPlayerStats>();
        if (p != null)
        {
            p.TotalEat();
            beliefs.ModifyState("LastAction", actionName);
            return true;
        }
        return false;
    }

    public override bool PrePerform()
    {
        Debug.Log($"[EatRestaurant] Canafford: {MoneyManager.Instance.CanAfford(MoneyCost)}"); ;
        if (!MoneyManager.Instance.CanAfford(MoneyCost))
        {
            if (!beliefs.HasState("NeedsMoney"))
            {
                GamePlayer player = GetComponentInParent<GamePlayer>();
                Debug.Log($"Cant afford eat restaurant => setting steal money goal");
                beliefs.AddState("NeedsMoney", true);
                player.AddGoal("StealMoney", 5, true);
            }
            return false;
        }
        target = BuildingManager.Instance.GetFirstBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.RESTAURANT).gameObject;
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        bool hasRestaurant = BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.RESTAURANT);
        bool hasMoney = MoneyManager.Instance.CanAfford(MoneyCost);
        return hasRestaurant && hasMoney;
    }
}
