using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBike : GAction
{
    public override bool PostPerform()
    {
        /*PlayerBike bike = GameObject.FindFirstObjectByType<PlayerBike>();
        if (bike == null)
        {
            return false;
        }
        bike.HopOn();*/
        return true;
    }

    public override bool PrePerform()
    {
        /*PlayerBike bike = GameObject.FindFirstObjectByType<PlayerBike>();
        if (bike != null)
        {
            target = bike.gameObject;
            return true;
        }*/
        return false;
    }
}
