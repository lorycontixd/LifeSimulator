using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform testTarget;

    private void Start()
    {/*
        agent = GetComponent<NavMeshAgent>();
        this.agent.updateRotation = false;
        this.agent.SetDestination(testTarget.position);*/
    }

    private void Update()
    {/*
        transform.LookAt(agent.velocity);
        transform.Rotate(-90, 0, -90);*/
    }
}
