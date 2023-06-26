using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using Lore.Game.Characters;

public class GoHome : GAction
{
    public override bool PostPerform()
    {
        if (beliefs.GetStates().ContainsKey("DogFollowing"))
        {
            Dog dog = FindFirstObjectByType<Dog>();
            if (dog != null)
            {
                dog.EndDogWalk();
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
