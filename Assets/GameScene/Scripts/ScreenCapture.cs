using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    public float ScreenCaptureCooldown = 2f;
    public bool DebugText;

    private float timestamp;
    private string SavePath;

    private void Start()
    {
        SavePath = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && Time.time > timestamp)
        {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot()
    {
        int count = CountFilesInSavePath();
        string filename = $"screeenshot{count}.png";
        string filepath = Path.Combine(SavePath, filename);
        UnityEngine.ScreenCapture.CaptureScreenshot(filepath);
        timestamp = Time.time + ScreenCaptureCooldown;
        if (DebugText)
        {
            Debug.Log($"[ScreenCapture] Saved screenshot in {filepath}", this);
        }
    }

    private int CountFilesInSavePath()
    {
        return Directory.GetFiles(SavePath).Length;
    }
}
