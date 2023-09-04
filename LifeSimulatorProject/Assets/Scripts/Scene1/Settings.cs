using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Setting
{
    public string key;
    public object value;
    public Type type;
}

public abstract class Settings
{
    public abstract Dictionary<string, object> ExportSettings();
}

public class SimulationSettings : Settings
{
    private int DayDurationSeconds = 200;
    private float SimulationSpeed = 1f;
    private bool IncludeBirds = true;

    public override Dictionary<string, object> ExportSettings()
    {
        return new Dictionary<string, object>() {
            { "Day duration", DayDurationSeconds },
            { "Simulation speed", SimulationSpeed },
            { "Include birds", IncludeBirds }
        };
    }
}

public class AudioSettings : Settings
{
    private bool IncludeMusic;
    private float Volume = 0.5f;

    public override Dictionary<string, object> ExportSettings()
    {
        return new Dictionary<string, object>() {
            {
                "Include music",
                IncludeMusic
            } ,
            {
                "Volume",
                Volume
            }
        };
    }
}

public class CameraSettings : Settings
{
    public float CameraHeight;

    public override Dictionary<string, object> ExportSettings()
    {
        return new Dictionary<string, object>()
        {
            {
                "Camera height",
                CameraHeight
            }
        };
    }
}