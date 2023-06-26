using GOAP;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyIngredients : GAction
{
    public override bool PostPerform()
    {
        return true;
    }

    public override bool PrePerform()
    {
        if (target == null && targetTag == string.Empty)
        {
            try
            {
                Building t = GameManager.Instance.GetClosestSupermarket(transform.position);
                target = t.gameObject;
            }
            catch
            {
                return false;
            }
        }
        return true;
    }
}
