using Lore.Game.Managers;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void SetMoney(float value)
    {
        if (moneyText == null) { return; }
        moneyText.text = $"Money: ${value}";
    }
    public void SetHealth(float value)
    {
        if (healthBar == null) { return; };
        healthBar.ChangeValue(value);
        healthBar.UpdateUI();
    }
}
