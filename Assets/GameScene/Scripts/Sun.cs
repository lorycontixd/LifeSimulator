using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Sun : MonoBehaviour
{
    [SerializeField] private Light sun;

    private float DayDurationSeconds;
    private float Timestamp;


    private void Start()
    {
        DayDurationSeconds = TimeManager.Instance.DayDurationMinutes * 60f;
    }

    private void Update()
    {
        Timestamp = TimeManager.Instance.DayPercentage;
        float yRot = Mathf.Lerp(0f, 360f, Timestamp);
        sun.transform.rotation = Quaternion.Euler(45f, yRot, 0f);
    }
}
