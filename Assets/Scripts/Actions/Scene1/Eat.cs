using GOAP;
using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    public override bool PostPerform()
    {
        this.GetComponent<NewPlayerStats>().Eat();
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
