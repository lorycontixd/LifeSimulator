using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDog : GAction
{
    private Dog dog;
    public override bool PostPerform()
    {
        beliefs.AddState("DogFollowing", true);
        return true;
    }

    public override bool PrePerform()
    {
        dog = FindFirstObjectByType<Dog>();
        if (dog == null) { return false; }
        target = dog.gameObject;
        return true;
    }

    public override bool IsAchievable()
    {
        return FindFirstObjectByType<Dog>() != null;
    }
}
