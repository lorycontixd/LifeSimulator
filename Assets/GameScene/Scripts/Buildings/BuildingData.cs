using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Buildings
{
    [CreateAssetMenu(fileName ="New Building", menuName = "Lore/Building")]
    public class BuildingData : ScriptableObject
    {
        #region BuildingType
        public enum BuildingType
        {
            NONE,
            HOUSE,
            COMPANY,
            HOSPITAL,
            SPORTS_CENTER,
            SUPERMARKET,
            MINIMARKET,
            BARRACKS,
            RESTAURANT,
            BANK,
            SCHOOL,
            PUB,
            ARCADE_ROOM
        }
        #endregion

        public int ID;
        public string Name;
        public BuildingType Type;
        public float ConstructionDuration;
        public float Cost;
        [Range(0f, 1f)] public float SellPercentage = 0.6f;
        public Sprite Icon;
        public GameObject buildingPrefab = null;
    }

}
    