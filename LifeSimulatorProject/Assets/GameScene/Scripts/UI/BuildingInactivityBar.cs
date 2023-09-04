using Lore.Game.Managers;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInactivityBar : MonoBehaviour
{
    [SerializeField] private ProgressBar bar;

    private bool IsActive = false;
    private float TimeRequired;
    private float Timestamp;

    private void Start()
    {
        BuildingManager.Instance.onBuildingInactivityWarning += StartCountdown;
        BuildingManager.Instance.onBuildingConstructed.AddListener(OnBuildingConstructed);

        TimeRequired = BuildingManager.Instance.BuildingInactivityTimeRequired;

        bar.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (IsActive && TimeManager.Instance.IsActive)
        {
            bar.ChangeValue(Timestamp);
            Timestamp -= Time.deltaTime;
        }
    }



    public void StartCountdown()
    {
        if (bar != null)
        {
            bar.gameObject.SetActive(true);
            bar.maxValue = TimeRequired;
            Timestamp = TimeRequired;
            IsActive = true;
        }
    }
    public void OnBuildingConstructed(Lore.Game.Buildings.Building b)
    {
        IsActive = false;
        bar.gameObject.SetActive(false);
    }
}
