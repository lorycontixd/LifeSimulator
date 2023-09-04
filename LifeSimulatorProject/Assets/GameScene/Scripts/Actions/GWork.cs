using GOAP;
using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Actions
{
    public class GWork : GAction
    {
        [SerializeField, Range(0f, 1f)] private float WorkProbabilityWhenSad = 0.35f;

        public override bool PostPerform()
        {
            beliefs.AddState("HasWorkedToday", true);
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.GetDailySalary();
            }
            if (beliefs.HasState("NeedsMoney"))
            {
                beliefs.RemoveState("NeedsMoney");
            }
            beliefs.ModifyState("LastAction", actionName);
            if (!beliefs.HasState("HasWorkedEver"))
            {
                beliefs.AddState("HasWorkedEver", true);
                if (Lore.Game.Managers.NotificationManager.Instance != null)
                {
                    Lore.Game.Managers.NotificationManager.Instance.Info("Remember to build", "You have more money now, you can construct buildings from the command panel");
                }
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
            Dog dog = GameObject.FindFirstObjectByType<Dog>();
            if (dog.state != Dog.DogState.IDLE)
            {
                dog.Reset();
            }
            return true;
        }

        public override bool IsAchievable()
        {
            bool rightTimeOfDay = (DayPart)GWorld.Instance.GetWorld().GetStates()["DayPart"] == DayPart.MORNING || (DayPart)GWorld.Instance.GetWorld().GetStates()["DayPart"] == DayPart.EVENING;
            bool hasWorkedToday = (bool)beliefs.HasState("HasWorkedToday");
            bool lastActionWasWork = ((string)beliefs.GetStates()["LastAction"] == actionName);
            bool isSick = SicknessManager.Instance.IsSick;
            bool dogFollowing = (bool)this.beliefs.HasState("DogFollowing");
            return rightTimeOfDay && !hasWorkedToday && !lastActionWasWork && !isSick && !dogFollowing && base.IsAchievable();
        }
    }

}
