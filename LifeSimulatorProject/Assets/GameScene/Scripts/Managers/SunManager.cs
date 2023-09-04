using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private Color morningLightColour;
    [SerializeField] private Color afternoonLightColour;
    [SerializeField] private Color eveningLightColour;
    [SerializeField] private Color nightLightColour;
    private Dictionary<DayPart, Color> sunColours = new Dictionary<DayPart, Color>();

    [SerializeField] private float colourChangeDuration;
    private bool isChangingColour;
    private Color currentColour;
    private Color targetColour;
    private float t;

    private void Start()
    {
        sunColours = new Dictionary<DayPart, Color>()
        {
            { DayPart.MORNING, morningLightColour },
            { DayPart.AFTERNOON, afternoonLightColour },
            { DayPart.EVENING, eveningLightColour },
            { DayPart.NIGHT, nightLightColour }
        };
        currentColour = sunColours[TimeManager.Instance.CurrentDayPart];
        TimeManager.Instance.onDayPartChange += OnDayPartChange;
    }
    private void Update()
    {
        if (isChangingColour)
        {
            if (t < colourChangeDuration)
            {
                Color lerpedColour = Color.Lerp(currentColour, targetColour, t / colourChangeDuration);
                sun.color = lerpedColour;
                t += Time.deltaTime;
            }
            else
            {
                isChangingColour = false;
                t = 0;
                currentColour = targetColour;
            }
        }
    }

    private void OnDayPartChange(DayPart oldValue, DayPart newValue)
    {
        switch (newValue)
        {
            case DayPart.MORNING:
            {
                isChangingColour = true;
                targetColour = morningLightColour;
                break;
            }
            case DayPart.AFTERNOON:
            {
                isChangingColour = true;
                targetColour= afternoonLightColour;
                break;
            }
            case DayPart.EVENING:
            {
                isChangingColour = true;
                    targetColour = eveningLightColour;
                break;
            }
            case DayPart.NIGHT:
            {
                isChangingColour = true;
                targetColour = nightLightColour;
                break;
            }
        }
    }


}
