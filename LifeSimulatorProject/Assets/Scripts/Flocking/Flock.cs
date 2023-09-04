using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float speed { get; private set; }

    private void Start()
    {


        speed = Random.Range(FlockManager.Instance.minSpeed, FlockManager.Instance.maxSpeed);
    }
    private void Update()
    {
        ApplyRules();
        this.transform.Translate(0, 0, speed*Time.deltaTime);
    }
    private void ApplyRules()
    {
        GameObject[] otherObjects = FlockManager.Instance.allObjects;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float groupSpeed = 0.01f;
        float neighbourDistance;
        int groupSize = 0;

        foreach(GameObject obj in otherObjects)
        {
            if (obj != this.gameObject)
            {
                neighbourDistance = Vector3.Distance(obj.transform.position, this.transform.position);
                if (neighbourDistance <= FlockManager.Instance.neighbourDistance) {
                    vCenter += obj.transform.position;
                    groupSize++;
                    if (neighbourDistance < 1.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - obj.transform.position);
                    }

                    Flock otherFlock = obj.GetComponent<Flock>();
                    groupSpeed = groupSpeed + otherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vCenter = vCenter / groupSize;
            speed = groupSpeed / groupSize;

            Vector3 direction = (vCenter + vAvoid) - transform.position; // New center for flock to aim
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.Instance.rotationSpeed * Time.deltaTime);
            }
        }
    }

}
