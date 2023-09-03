using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentDayPartText;
    [SerializeField] TextMeshProUGUI gameTimeText;
    [SerializeField] TextMeshProUGUI daysPassedText;


    private void Update()
    {
        currentDayPartText.text = $"Day part: {TimeManager.Instance.CurrentDayPart}";
        gameTimeText.text = $"Game time: {TimeManager.Instance.TimeSinceStart}";
        daysPassedText.text = $"{TimeManager.Instance.DaysPassed} days passed";
    }
}
