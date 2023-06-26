using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBaker : MonoBehaviour
{

    public NavMeshSurface[] surfaces;
    public Transform[] objectsToRotate;

    [Header("Settings")]
    [SerializeField] private bool AutoFetchSurfaces;

    // Use this for initialization
    void Start()
    {
        if (AutoFetchSurfaces)
        {
            surfaces = GameObject.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None).ToArray();
        }
        Bake();
    }

    public void Bake()
    {
        for (int j = 0; j < objectsToRotate.Length; j++)
        {
            objectsToRotate[j].localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        }

        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

}