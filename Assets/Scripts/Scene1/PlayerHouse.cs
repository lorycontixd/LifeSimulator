using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHouse: MonoBehaviour
{
    [SerializeField] private BoxCollider region = null;

    private void Start()
    {
        if (region == null)
            region = GetComponent<BoxCollider>();
    }

}
