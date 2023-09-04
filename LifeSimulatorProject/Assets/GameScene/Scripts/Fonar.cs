using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fonar : MonoBehaviour
{
    private Light fonarLight;

    private void Start()
    {
        if (fonarLight == null)
        {
            fonarLight = GetComponentInChildren<Light>();
        }
        fonarLight.enabled = false;
        TimeManager.Instance.onDayPartChange += OnDayPartChange;
    }

    private void OnDayPartChange(DayPart part1, DayPart part2)
    {
        if (part2 == DayPart.EVENING || part2 == DayPart.NIGHT)
        {
            fonarLight.enabled = true;
        }
        else
        {
            fonarLight.enabled = false;
        }
    }
}
