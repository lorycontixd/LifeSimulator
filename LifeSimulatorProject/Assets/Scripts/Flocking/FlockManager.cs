using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    #region Singleton
    private static FlockManager _instance;
    public static FlockManager Instance { get { return _instance; } }

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

    [SerializeField] private GameObject objectPrefab;
    public int objectCount;
    public GameObject[] allObjects { get; private set; }
    public Vector3 regionBounds = new Vector3(5, 5, 5);

    [Header("Fish settings")]
    [Range(0f, 5f)] public float minSpeed;
    [Range(0f, 5f)] public float maxSpeed;
    [Range(1f, 10f), Tooltip("Distance between objects that adds a moving behaviour to avoid crowding.")] public float neighbourDistance;
    [Range(1f, 5f)] public float rotationSpeed;


    private void Start()
    {
        allObjects = new GameObject[objectCount];
        for(int i = 0; i < allObjects.Length; i++)
        {
            float x = Random.Range(-regionBounds.x, regionBounds.x);
            float y = Random.Range(-regionBounds.y, regionBounds.y);
            float z = Random.Range(-regionBounds.z, regionBounds.z);
            Vector3 pos = transform.position + new Vector3(x, y, z);

            allObjects[i] = Instantiate(objectPrefab, pos, Quaternion.identity, this.transform);
        }
    }
    private void Update()
    {
        PrintDebug();
    }
    private void PrintDebug()
    {
        List<Vector3> positions = allObjects.Select(o => o.transform.position).ToList();
        Vector3 center = Vector3.zero;
        foreach(Vector3 pos in positions)
        {
            center += pos;
        }
        center = center / positions.Count;

        float groupAvgSpeed = allObjects.Sum(o => o.GetComponent<Flock>().speed) / allObjects.Length;

        Debug.Log($"Group center: {center},   Group avg speed: {groupAvgSpeed}", this);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(this.transform.position, regionBounds);
    }
}
