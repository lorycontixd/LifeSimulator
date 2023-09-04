using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock2 : MonoBehaviour {

    float speed;
    bool turning = false;

    void Start() {

        speed = Random.Range(FlockManager2.Instance.minSpeed, FlockManager2.Instance.maxSpeed);
    }


    void Update() {

        Bounds b = new Bounds(FlockManager2.Instance.transform.position, FlockManager2.Instance.regionLimits * 2.0f);

        if (!b.Contains(transform.position)) {

            turning = true;
        } else {

            turning = false;
        }

        if (turning) {

            Vector3 direction = FlockManager2.Instance.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                FlockManager2.Instance.rotationSpeed * Time.deltaTime);
        } else {


            if (Random.Range(0, 100) < 10) {

                speed = Random.Range(FlockManager2.Instance.minSpeed, FlockManager2.Instance.maxSpeed);
            }


            if (Random.Range(0, 100) < 10) {
                ApplyRules();
            }
        }

        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }

    private void ApplyRules() {

        GameObject[] gos;
        gos = FlockManager2.Instance.allObjects;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float gSpeed = 0.01f;
        float mDistance;
        int groupSize = 0;

        foreach (GameObject go in gos) {

            if (go != this.gameObject) {

                mDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (mDistance <= FlockManager2.Instance.neighbourDistance) {

                    vCentre += go.transform.position;
                    groupSize++;

                    if (mDistance < 1.0f) {

                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flock2 anotherFlock = go.GetComponent<Flock2>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0) {

            vCentre = vCentre / groupSize + (FlockManager2.Instance.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockManager2.Instance.maxSpeed) {

                speed = FlockManager2.Instance.maxSpeed;
            }

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if (direction != Vector3.zero) {

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    FlockManager2.Instance.rotationSpeed * Time.deltaTime);
            }
        }
    }
}