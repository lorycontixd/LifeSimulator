using Lore.Game.Managers;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        SetLevel(BuildingManager.Instance.CityLevel);
        BuildingManager.Instance.onLevelUpgrade.AddListener(SetLevel);
        // Initial money text set in UIManager
    }
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }
    public void SetMoney(float value)
    {
        if (moneyText == null) { return; }
        string limitedVal = value.ToString("0.00");
        moneyText.text = $"Money: ${limitedVal}";
    }
    public void SetHealth(float value)
    {
        if (healthBar == null) { return; };
        healthBar.ChangeValue(value);
        healthBar.UpdateUI();
    }
}
