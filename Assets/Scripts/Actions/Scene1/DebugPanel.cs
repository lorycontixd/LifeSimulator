using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI frameCountText;
    [SerializeField] private TextMeshProUGUI maxDeltaTimeText;
    [SerializeField] private TextMeshProUGUI currentActionText;
    [SerializeField] private TextMeshProUGUI availableActionsText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI distanceToHouseText;
    [SerializeField] private float _hudRefreshRate = 1f;
    private float _timer;
    private Player player;

    private void Start()
    {
        player = GameObject.FindFirstObjectByType<Player>();
    }
    private void Update()
    {
        UpdateTexts();
    }
    private void UpdateTexts()
    {
        FPSText();
        FrameCount();
        MaxDeltaTime();
        DistanceTraveled();
        DistanceToPlayerHouseText();
        CurrentActionText();
    }

    private void FPSText()
    {
        if (fpsText != null)
        {
            if (Time.unscaledTime > _timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                fpsText.text = "FPS: " + fps;
                _timer = Time.unscaledTime + _hudRefreshRate;
            }
        }
    }
    private void FrameCount()
    {
        if (frameCountText != null)
        {
            frameCountText.text = $"Frame count: {Time.frameCount}";
        }
    }
    private void MaxDeltaTime()
    {
        if (maxDeltaTimeText != null)
        {
            maxDeltaTimeText.text = $"Max delta time: {Time.maximumDeltaTime}";
        }
    }
    private void DistanceTraveled()
    {
        if (distanceText != null)
        {
            if (player != null)
            {
                float dist = player.GetComponent<DistanceTraveled>().GetDistanceTraveled();
                distanceText.text = $"Distance traveled: {dist}";
            }
        }
    }
    private void DistanceToPlayerHouseText()
    {
        if (distanceToHouseText != null)
        {
            if (player != null)
            {
                float dist = player.GetComponent<DistanceTraveled>().GetDistanceFromPlayerHouse();
                distanceToHouseText.text = $"Distance from house: {dist}";
            }
        }
    }
    public void CurrentActionText()
    {
        if (currentActionText != null)
        {
            if (player != null)
            {
                if (player.currentAction != null)
                {
                    currentActionText.text = $"Current Action: {player.currentAction.actionName}";
                }
                else
                {
                    currentActionText.text = "No action";
                }
            }
        }
    }
}
