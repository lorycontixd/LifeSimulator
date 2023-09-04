using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewBuilding = Lore.Game.Buildings.Building;

public class StealMoney : GAction
{
    [SerializeField] private Vector2 stealRange;

    [SerializeField, Tooltip("Steal duration is inversely proportional to the stealing amount")]
    private Vector2 durationRange;

    [SerializeField, Tooltip("Probability of stealing from a bank if built"), Range(0f,1f)]
    private float bankStealProbability = 0.4f;

    [SerializeField, Tooltip("Multiplier if stealing from a bank"), Range(1f, 3f)]
    private float bankStealMultiplier = 2.1f;

    [SerializeField, Tooltip("How much the player is slowed down during the theft"), Range(1f, 3f)]
    private float speedReductionFactor = 2.1f;

    [SerializeField, Tooltip("Send notification if successful")]
    private bool SendNotification = true;


    private NewBuilding targetBuilding;
    private float stealAmount = -1f;
    private GamePlayer player;


    public override bool PostPerform()
    {
        player.SetStealing(false);
        player.ResetNavmeshSpeed();
        targetBuilding = null;
        if (NotificationManager.Instance != null && SendNotification)
        {
            NotificationManager.Instance.Info($"Theft successful", $"You successfully stole {stealAmount} money");
        }
        if (beliefs.HasState("IsStealing"))
        {
            beliefs.RemoveState("IsStealing");
        }
        if (beliefs.HasState("NeedsMoney"))
        {
            beliefs.RemoveState("NeedsMoney");
        }
        beliefs.ModifyState("LastAction", actionName);
        stealAmount = -1f;
        return true;
    }

    public override bool PrePerform()
    {
        bool bankBuilt = BuildingManager.Instance.IsBuildingConstructed(Lore.Game.Buildings.BuildingData.BuildingType.BANK);
        if (!bankBuilt)
        {
            targetBuilding = BuildingManager.Instance.GetRandomBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.HOUSE);
        }
        else
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r < bankStealMultiplier)
            {
                targetBuilding = BuildingManager.Instance.GetRandomBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.BANK);
            }
            else
            {
                targetBuilding = BuildingManager.Instance.GetRandomBuildingByType(Lore.Game.Buildings.BuildingData.BuildingType.HOUSE);
            }
        }
        bool isBank = targetBuilding.data.Type == Lore.Game.Buildings.BuildingData.BuildingType.BANK;
        string targetStr = isBank ? "bank" : "house";
        float multiplier = isBank ? bankStealMultiplier : 1f;
        stealAmount = Random.Range(stealRange.x, stealRange.y);
        duration = UtilityFunctions.ConvertRange(stealRange.x, stealRange.y, durationRange.x, durationRange.y, stealAmount);
        target = targetBuilding.gameObject;
        stealAmount = stealAmount * multiplier;
        // Set player stealing
        player = GetComponentInParent<GamePlayer>();
        player.SetStealing(true);
        if (!beliefs.HasState("IsStealing"))
        {
            beliefs.AddState("IsStealing", true);
        }
        player.SetNavmeshSpeed(player.GetNavmeshSpeed() / speedReductionFactor);
        if (NotificationManager.Instance != null && SendNotification)
        {
            NotificationManager.Instance.Info($"Entered steal mode", $"Going to steal money from {targetStr}. You are now detectable by policement.");
        }
        return true;
    }

    public override bool IsAchievable()
    {
        bool hasDog = beliefs.HasState("DogFollowing");
        bool dogWantsWalk = beliefs.HasState("DogWantsWalk");
        bool hasWorked = beliefs.HasState("HasWorkedToday");
        bool isStealing = beliefs.HasState("IsStealing");
        bool baseVal = base.IsAchievable();
        return !hasDog && !dogWantsWalk && hasWorked && !isStealing && baseVal;
    }
}
