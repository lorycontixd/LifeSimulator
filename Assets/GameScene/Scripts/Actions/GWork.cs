using GOAP;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Actions
{
    public class GWork : GAction
    {
        public override bool PostPerform()
        {
            beliefs.AddState("HasWorkedToday", true);
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.GetDailySalary();
            }
            return true;
        }

        public override bool PrePerform()
        {
            Lore.Game.Buildings.Building b = BuildingManager.Instance.WorkBuilding;
            if (b == null)
            {
                return false;
            }
            target = b.gameObject;
            return true;
        }

        public override bool IsAchievable()
        {
            bool rightTimeOfDay = (DayPart)GWorld.Instance.GetWorld().GetStates()["DayPart"] == DayPart.MORNING || (DayPart)GWorld.Instance.GetWorld().GetStates()["DayPart"] == DayPart.EVENING;
            bool hasWorkedToday = (bool)beliefs.HasState("HasWorkedToday");
            return rightTimeOfDay && !hasWorkedToday;
        }
    }

}
