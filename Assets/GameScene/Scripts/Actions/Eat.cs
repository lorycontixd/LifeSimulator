using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Actions
{
    public class Eat : GAction
    {
        public override bool PostPerform()
        {
            return true;
        }

        public override bool PrePerform()
        {
            return false;
        }
    }
}
