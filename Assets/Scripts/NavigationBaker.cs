using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavigationBaker : MonoBehaviour
{
    private void Start()
    {
        NavMeshBuildSettings settings = new NavMeshBuildSettings();
        settings.agentHeight = 2f;
        settings.agentRadius = 0.4f;
        settings.agentSlope = 50f;
        settings.agentClimb = 0.4f;

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        sources.Add(new NavMeshBuildSource());
        UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(settings, sources, new Bounds(), Vector3.zero, Quaternion.identity);

    }
}