using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool isTaken = false;

    public void SetState(bool isTaken)
    {
        this.isTaken = isTaken;
    }
}
