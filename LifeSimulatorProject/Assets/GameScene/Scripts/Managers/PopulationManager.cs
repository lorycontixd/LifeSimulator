using Lore.Game.Characters;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{

}
public class PopulationManager : BaseManager
{
    #region Spawnpoint Selector Mode
    [Serializable]
    public enum SpawnpointSelectorMode
    {
        FIRST,
        RANDOM,
        CLOSEST
    }
    #endregion

    #region Singleton
    private static PopulationManager _instance;
    public static PopulationManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public bool SpawnOnHouseConstructed = true;
    public int SpawnEveryHouses = 2;
    public GameObject citizenPrefab = null;
    [SerializeField] private SpawnpointSelectorMode spawnpointSelectionMode;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private Transform cloneHolder = null;

    public int TotalPopulation { get; private set; }

    private List<Character> spawnedCharacters = new List<Character>();
    private List<Citizen> spawnedCitizens = new List<Citizen>();
    private int currentHouseCount;


    public override void Start()
    {
        BuildingManager.Instance.onBuildingConstructed.AddListener(OnBuildingContructed);
        base.Start();
        TotalPopulation = 0;
        currentHouseCount = BuildingManager.Instance.HousesCount;

    }

    private void OnBuildingContructed(Lore.Game.Buildings.Building building)
    {
        currentHouseCount++;
        if (SpawnOnHouseConstructed)
        {
            if (building.data.Type == Lore.Game.Buildings.BuildingData.BuildingType.HOUSE)
            {
                if (building.state == Lore.Game.Buildings.Building.BuildingState.BUILT)
                {
                    Transform sp = PickSpawnpoint();
                    if (sp != null)
                    {
                        if (currentHouseCount % SpawnEveryHouses == 0)
                        {
                            SpawnCitizen(sp);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Could not instantiate targetCitizen because no spawnpoint was found");
                        return;
                    }
                }
            }
        }
    }

    private Transform PickSpawnpoint()
    {
        Transform result = null;
        switch(spawnpointSelectionMode)
        {
            case SpawnpointSelectorMode.FIRST:
                if (spawnPoints.Count > 0)
                    result = spawnPoints[0];
                break;
            case SpawnpointSelectorMode.RANDOM:
                if (spawnPoints.Count > 0)
                {
                    result = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count - 1)];
                }
                break;
            case SpawnpointSelectorMode.CLOSEST:
                if (spawnPoints.Count > 0)
                {
                    Vector3 playerPos = FindFirstObjectByType<GamePlayer>().GetComponent<Transform>().position;
                    Transform closest = null;
                    foreach(Transform t in spawnPoints)
                    {
                        if (closest == null)
                        {
                            closest = t;
                        }
                        else
                        {
                            if (Vector3.Distance(playerPos,t.position) < Vector3.Distance(playerPos, closest.position))
                            {
                                closest = t;
                            }
                        }
                    }
                    if (closest != null)
                    {
                        result = closest;
                    }
                }
                break;
        }
        return result;
    }
    public void SpawnCitizen(Transform spawnpoint)
    {
        GameObject clone = Instantiate(citizenPrefab, spawnpoint.position, spawnpoint.rotation, (cloneHolder != null) ? cloneHolder : transform);
        Citizen citizen = clone.GetComponent<Citizen>();
        if (citizen != null)
        {
            citizen.wanderMode = Citizen.WanderMode.STEERING;
            Debug.Log($"Created new targetCitizen at {spawnpoint.position} => {citizen.ID}");
        }
        else
        {
            Destroy(clone);
        }
    }
    public void AddCitizen(Citizen citizen)
    {
        spawnedCharacters.Add(citizen);
        spawnedCitizens.Add(citizen);
        TotalPopulation++;
    }
}
