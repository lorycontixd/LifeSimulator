using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    NONE,
    SUPERMARKET,
    COMPANY,
    WORK
}

[System.Serializable]
public struct BuildingSetup
{
    public BuildingType type;
    public int count;
    public bool canWalkInside;
}

public class Building : MonoBehaviour
{
    public int id;
    public BuildingType buildingType;

}
