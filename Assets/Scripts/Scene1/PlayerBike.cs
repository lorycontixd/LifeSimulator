using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBike : MonoBehaviour
{
    private Vector3 restPosition;
    public bool IsAtRest { get { return Vector3.Distance(restPosition, transform.position) <= 3f; } }
    public bool IsPlayerOn { get;private set; }

    private void Start()
    {
        restPosition = transform.position;
    }

    public void HopOn()
    {
        IsPlayerOn= true;
    }
    public void HopOff()
    {
        IsPlayerOn = false;
    }


}
