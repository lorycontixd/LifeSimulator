using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTraveled : MonoBehaviour
{
    [SerializeField] private bool debug = true;
    public bool IsDistanceTraveledActive { get; private set; } = true;
    private Player player = null;
    private float totalDistanceTraveled = 0f;
    private Vector3 previousPosition;
    private float totalDistanceToPlayerHouse;

    private Transform playerHouse;


    private void Awake()
    {
        player = GameObject.FindFirstObjectByType<Player>();
        playerHouse = GameObject.FindGameObjectWithTag("PlayerHouse").transform;
    }
    private void Start()
    {
        previousPosition = transform.position;
    }
    void Update()
    {
        if (IsDistanceTraveledActive)
        {
            float distanceThisFrame = Vector3.Distance(transform.position, previousPosition);
            totalDistanceTraveled += distanceThisFrame;
            if (debug)
                Debug.Log("Distance this frame: " + distanceThisFrame.ToString() + ",   Total Distance: " + totalDistanceTraveled.ToString());
            previousPosition = transform.position;
        }
        totalDistanceToPlayerHouse = Vector3.Distance(transform.position, playerHouse.position);
    }

    public void SetDistanceTraveledActive(bool isactive)
    {
        this.IsDistanceTraveledActive = isactive;
    }

    public float GetDistanceTraveled()
    {
        return totalDistanceTraveled;
    }
    public float GetDistanceFromPlayerHouse()
    {
        return totalDistanceToPlayerHouse;
    }
}
