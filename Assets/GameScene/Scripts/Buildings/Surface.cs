using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Surface : MonoBehaviour
{
    public bool IsBuildable = false;
    public bool IsWalkable = false;

    private NavMeshSurface navmeshSurface;

    private void Start()
    {
        if (navmeshSurface == null)
        {
            //navmeshSurface = gameObject.AddComponent<NavMeshSurface>();
            //navmeshSurface.defaultArea = 0;
        }
        gameObject.layer = 6;
    }
}
