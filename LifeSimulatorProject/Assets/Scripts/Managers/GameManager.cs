using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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


    [Header("Prefabs")]
    public GameObject buildingCanvasPrefab = null;
    [SerializeField] private List<GameObject> npcPrefabs = new List<GameObject>();
    [SerializeField] private GameObject debugSphere = null;

    [Header("Crowd Settings")]
    [SerializeField] private Transform mapCenter = null;
    [SerializeField] private float npcSpawnRadius = 60f;
    [SerializeField] private Transform npcParent = null;
    [SerializeField] private int npcAreaMaskIndex = 0;

    [Header("Buildings Settings")]
    [SerializeField] private List<BuildingSetup> buildingCounts;
    [SerializeField] private int npcCount;

    [Header("Settings")]
    [SerializeField] private bool debugMode;

    public bool GameSetup { get; private set; }
    private List<Building> buildings;
    private System.Random rnd;
    private Dictionary<Personality, int> personalitiesSpawned = new Dictionary<Personality, int>();


    private void Start()
    {
        rnd = new System.Random();
        SetupBuildings();
        SetupCrowd();
        GameSetup = true;
    }
    private void Update()
    {
        
    }

    private void SetupBuildings()
    {
        buildings = GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        // Set ids
        int index = 0;
        foreach (Building building in buildings)
        {
            if (building != null)
            {
                building.id = index;
                building.buildingType = BuildingType.NONE;
            }
            index++;
        }

        var buildingTypes = Enum.GetValues(typeof(BuildingType));
        foreach(var buildingType in buildingTypes)
        {
            BuildingType bType = (BuildingType)buildingType;
            if (bType == BuildingType.NONE)
            {
                continue;
            }
            // Select all free buildings that have none as building type
            var freeBuildings = buildings.Where(b => b.buildingType == BuildingType.NONE).ToList();
            List<BuildingSetup> counts = buildingCounts.Where(b => b.type == bType).ToList(); // 1-element list
            if (counts.Count > 1)
            {
                Debug.LogError($"More than one building count declaration for type {bType} was found.");
                return;
            }
            if (counts.Count > 0)
            {
                BuildingSetup setup = counts[0];
                int count = setup.count;
                if (count > freeBuildings.Count)
                {
                    Debug.LogError($"More buildings required than available. Capping it to number of available buildings.");
                    count = freeBuildings.Count;
                }

                List<Building> selectedBuildings = freeBuildings.OrderBy(x => rnd.Next()).Take(count).ToList();
                foreach (Building b in selectedBuildings)
                {
                    b.buildingType = bType;
                    b.gameObject.tag = bType.ToString().ToLower().Capitalize();
                    b.GetComponent<NavMeshObstacle>().enabled = false;
                    SpawnBuildingCanvas(b.transform, bType.ToString().ToLower().Capitalize());
                    if (setup.canWalkInside)
                    {
                        b.GetComponentInParent<NavMeshObstacle>().enabled = false;
                    }
                }
            }
        }

        Building workBuilding = SelectWorkCompany();
        SpawnBuildingCanvas(workBuilding.transform, "GWork", Color.red);

    }

    private void SetupCrowd()
    {
        for (int i=0; i < npcCount; i++)
        {
            int prefabIndex = Random.Range(0, npcPrefabs.Count - 1);
            Vector3 position = SamplePositionFromNavmeshArea(mapCenter != null ? mapCenter.position : Vector3.zero, npcSpawnRadius, npcAreaMaskIndex);
            GameObject clone = Instantiate(npcPrefabs[prefabIndex], position, Quaternion.identity, npcParent);
            NPC npc = clone.GetComponent<NPC>();
            if (npc == null)
            {
                Debug.LogWarning($"Spawned npc object without npc component. Adding new one");
                clone.gameObject.AddComponent<NPC>();
            }
            // NPC parameters
            int id = i;
            int personalityIndex = Random.Range(0, Enum.GetValues(typeof(Personality)).Length - 1);
            Personality personality = (Personality)Enum.GetValues(typeof(Personality)).GetValue(personalityIndex);
            npc.SetupNPC(id, personality);
            // Add a count to the Personality spawned
            if (!personalitiesSpawned.ContainsKey(personality))
            {
                personalitiesSpawned.Add(personality, 1);
            }
            else
            {
                personalitiesSpawned[personality] += 1;
            }
        }
    }

    public static Vector3 SamplePositionFromNavmeshArea(Vector3 center, float radius = 50f, int mask = 0)
    {
        Vector2 randomDir = Random.insideUnitCircle;
        Vector3 point = new Vector3(randomDir.x, 0.5f, randomDir.y) * radius;
        NavMeshHit hit;
        NavMesh.SamplePosition(point, out hit, radius, mask);
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }

    public void SpawnBuildingCanvas(Transform building, string label, Color? color = null)
    {
        if (buildingCanvasPrefab != null)
        {
            GameObject clone = Instantiate(buildingCanvasPrefab, building.transform);
            TextMeshProUGUI textComponent = clone.GetComponentInChildren<TextMeshProUGUI>(); 
            textComponent.text = label;
            if (color != null)
            {
                textComponent.color = color.Value;
            }
            else
            {
                textComponent.color = Color.white;
            }
        }
    }


    public Building GetRandomBuilding()
    {
        return buildings.OrderBy(x => rnd.Next()).Take(1).ToList()[0];
    }
    public Building GetRandomSupermarket()
    {
        var supermarkets = buildings.Where(b => b.buildingType == BuildingType.SUPERMARKET).OrderBy(x => rnd.Next()).Take(1).ToList();
        if (supermarkets.Count > 0) {
            return supermarkets[0];
        }
        else
        {
            return null;
        }
    }
    public Building GetClosestSupermarket(Vector3 position)
    {
        var supermarkets = buildings.Where(b => b.buildingType == BuildingType.SUPERMARKET).ToList();
        Building closest = null;
        foreach(Building b in supermarkets)
        {
            if (closest == null || Vector3.Distance(position, b.transform.position) < Vector3.Distance(position, closest.transform.position))
            {
                closest = b;
            }
        }
        return closest;
    }


    public Building SelectWorkCompany()
    {
        var companies = buildings.Where(b => b.buildingType == BuildingType.COMPANY).ToList();
        List<Building> selectedBuildings = companies.OrderBy(x => rnd.Next()).Take(1).ToList();
        if (selectedBuildings.Count > 0)
        {
            Building workBuilding = selectedBuildings[0];
            if (workBuilding.GetComponentInChildren<Canvas>() != null)
            {
                Destroy(workBuilding.GetComponentInChildren<Canvas>().gameObject );
            }
            workBuilding.tag = "GWork";
            workBuilding.buildingType = BuildingType.WORK;
            return workBuilding;
        }
        else
        {
            Debug.LogWarning($"[GameManager] Tried to fetch work company but it was not possible. No buildings of type company were found.", this);
            return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mapCenter != null && npcSpawnRadius > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(mapCenter.position, npcSpawnRadius);
        }
    }
}
