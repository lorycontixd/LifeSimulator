using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GAgent
{
    [Header("Fatigue Settings")]
    public float MaxFatigue = 6f;
    [Range(0.1f, 1f)] public float RestThresholdPerc = 0.7f; // Percentage of maximum fatigue when nurse is declared exhausted
    public float Fatigue { get; private set; } = 0;

    protected override void Start()
    {
        base.Start();

        Goal s1 = new Goal("TreatPatient", 1, false);
        goals.Add(s1, 4);

        Goal s2 = new Goal("TotalRest", 1, false);
        goals.Add(s2, 5);
    }

    public void GetTired(float value)
    {
        Fatigue = Mathf.Clamp(Fatigue + value, 0, MaxFatigue);
        if (Fatigue > RestThresholdPerc * MaxFatigue)
        {
            this.beliefs.ModifyState("CanRest", true);
        }
    }
    public void Rest()
    {
        Fatigue = 0f;
        beliefs.RemoveState("CanRest");
    }
}