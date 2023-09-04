using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Investmenet", menuName = "Lore/Investment")]
public class Investment : ScriptableObject
{
    public int ID;
    public string Name;
    public float Cost;
    public float MonthlyPassiveIncome;
    public bool IsLocked = false;

    public void Setup(int ID)
    {
        this.ID = ID;
    }
}
