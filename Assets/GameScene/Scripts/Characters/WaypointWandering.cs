using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WaypointWandering : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float arrivalDistance = 1.5f;
    public bool IsActive { get; private set; } = false;

    public Action<Transform> onWaypointArrived;

    private int waypointIndex = 0;
    [SerializeField] NavMeshAgent agent;

    private void Start()
    {
        if (waypoints.Count < 2)
        {
            Debug.LogWarning($"Must have at least 2 waypoints for wandering to work, but recevied {waypoints.Count}");
            IsActive = false;
        }
    }
    private void Update()
    {
        if (agent != null && IsActive)
        {
            if (Vector3.Distance(agent.transform.position, waypoints[waypointIndex].position) < arrivalDistance)
            {
                OnWaypointArrived();
            }
        }
    }
    public void SetAgent(NavMeshAgent agent) {
        this.agent= agent;
    }
    public void Activate()
    {
        IsActive = true;
    }
    public void Deactivate()
    {
        IsActive= false;
    }
    
    public int NextWaypointIndex()
    {
        return (waypointIndex + 1) % waypoints.Count;
    }
    private int RandomWaypointIndex()
    {
        var exclude = new HashSet<int>() { waypointIndex };
        var range = Enumerable.Range(0, waypoints.Count).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(0, waypoints.Count - exclude.Count);
        return range.ElementAt(index);
    }
    public void OnWaypointArrived()
    {
        onWaypointArrived?.Invoke(waypoints[waypointIndex]);
        waypointIndex = RandomWaypointIndex();
        agent.SetDestination(waypoints[waypointIndex].position);
    }
}
