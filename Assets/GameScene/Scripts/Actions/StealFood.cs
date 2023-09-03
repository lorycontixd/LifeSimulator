using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealFood : GAction
{
    [SerializeField] private float speedReductionFactor = 2.1f;
    [SerializeField] private bool SendNotification = true;

    private GamePlayer player;


    public override bool PostPerform()
    {
        player.SetStealing(false);
        player.ResetNavmeshSpeed();
        NewPlayerStats stats = player.GetComponent<NewPlayerStats>();
        stats.TotalEat();
        if (NotificationManager.Instance != null && SendNotification)
        {
            NotificationManager.Instance.Info($"Theft successful", "You successfully stole food");
        }
        if (beliefs.HasState("IsStealing"))
        {
            beliefs.RemoveState("IsStealing");
        }
        return true;
    }

    public override bool PrePerform()
    {
        target = BuildingManager.Instance.GetRandomBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.HOUSE).gameObject;
        player = GetComponentInParent<GamePlayer>();
        player.SetStealing(true);
        if (!beliefs.HasState("IsStealing"))
        {
            beliefs.AddState("IsStealing", true);
        }
        player.SetNavmeshSpeed(player.GetNavmeshSpeed() / speedReductionFactor);
        if (NotificationManager.Instance != null && SendNotification)
        {
            NotificationManager.Instance.Info($"Entered steal mode", "Going to steal food. You are now detectable by policement.");
        }
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        bool isSupermarketBuilt = BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.SUPERMARKET);
        if (isSupermarketBuilt) { return false; }
        return base.IsAchievableGiven (conditions);
    }
}
