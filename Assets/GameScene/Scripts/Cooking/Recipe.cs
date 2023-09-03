using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="New Recipe", menuName = "Lore/Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public enum RecipeCourse
    {
        UNKNOWN,
        MAIN,
        SECOND,
        DESSERT,
        FRUIT
    }

    public string Name;
    public List<Ingredient> Ingredients;
    public float CookingDuration;
    public float HungerDecrease;
    public RecipeCourse Course = RecipeCourse.UNKNOWN;

    public bool HasIngredients(List<Ingredient> list)
    {
        foreach(Ingredient i in Ingredients)
        {
            if (!list.Contains(i))
            {
                return false;
            }
        }
        return true;
    }

    public float TotalCost()
    {
        return Ingredients.Sum(i => i.Cost);
    }

}
