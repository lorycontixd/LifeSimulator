using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Ingredient", menuName = "Lore/Cooking/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string Name;
    public float Cost;
}
