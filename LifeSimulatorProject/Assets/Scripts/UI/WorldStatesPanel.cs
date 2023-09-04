using GOAP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldStatesPanel : MonoBehaviour
{
    [SerializeField] private Transform parent = null;
    [SerializeField] private GameObject statePrefab = null;

    private void Start()
    {
        if (parent == null)
            parent = transform;
        GWorld.Instance.GetWorld().onWorldStateChange += OnWorldStateChange;
        ShowStates();

    }

    private void Update()
    {
    }

    private void ClearStates()
    {
        for (int i = 0; i < parent.childCount; i++) {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    private void SpawnState(KeyValuePair<string, object> kvp)
    {
        GameObject clone = Instantiate(statePrefab, parent);
        TextMeshProUGUI text = clone.GetComponent<TextMeshProUGUI>();
        text.text = $"{kvp.Key} - {kvp.Value}";
    }
    private void SpawnTitle()
    {
        GameObject clone = Instantiate(statePrefab, parent);
        TextMeshProUGUI text = clone.GetComponent<TextMeshProUGUI>();
        text.text = "World States";
        text.color = Color.red;
    }
    private void ShowStates()
    {
        if (statePrefab == null)
            return;
        ClearStates();
        SpawnTitle();
        foreach (KeyValuePair<string, object> kvp in GWorld.Instance.GetWorld().GetStates())
        {
            SpawnState(kvp);
        }
    }

    public void OnWorldStateChange(WorldStates.WorldStateChangeType type, string key)
    {
        ShowStates();
    }
}
