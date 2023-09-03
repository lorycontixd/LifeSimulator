using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Disease", menuName ="Lore/Disease")]
public class Disease : ScriptableObject
{
    public string Name;
    [Range(1f, 20f)] public float ActiveAfterSeconds = 5f;
    [Range(0.1f, 5f)] public float HealthLossRatePerSec = 0.5f;
    [Range(0f, 1f)] public float DailyProbabilityOfEncounter = 0.1f;
    [Range(1f, 10f)] public float CureDuration = 4f;
    public bool IsActive { get; private set; } = false;


    public void OnEnable()
    {
        IsActive = false;
    }
    public void Activate() { IsActive = true; }
}
