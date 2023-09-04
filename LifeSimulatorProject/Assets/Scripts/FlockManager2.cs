using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager2 : MonoBehaviour {

    public static FlockManager2 Instance;
    public GameObject objectPrefab;
    public int numObjects = 20;
    public GameObject[] allObjects;
    public Vector3 regionLimits = new Vector3(5.0f, 5.0f, 5.0f);
    public Vector3 goalPos = Vector3.zero;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed;
    [Range(0.0f, 5.0f)] public float maxSpeed;
    [Range(1.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 5.0f)] public float rotationSpeed;

    void Start() {

        allObjects = new GameObject[numObjects];

        for (int i = 0; i < numObjects; ++i) {

            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-regionLimits.x, regionLimits.x),
                Random.Range(-regionLimits.y, regionLimits.y),
                Random.Range(-regionLimits.z, regionLimits.z));

            allObjects[i] = Instantiate(objectPrefab, pos, Quaternion.identity, this.transform);
        }

        Instance = this;
        goalPos = this.transform.position;
    }


    void Update() {

        if (Random.Range(0, 100) < 10) {

            goalPos = this.transform.position + new Vector3(
                Random.Range(-regionLimits.x, regionLimits.x),
                Random.Range(-regionLimits.y, regionLimits.y),
                Random.Range(-regionLimits.z, regionLimits.z));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, regionLimits);
    }
}