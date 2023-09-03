using Lore.Game.Buildings;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InvestmentPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI monthlyIncomeText;

    private Investment investment;
    private Button button;

    public UnityEvent<InvestmentPanelItem> onBuySuccess;
    public UnityEvent<InvestmentPanelItem, MoneyManager.PurchaseFailedReason> onBuyFail;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }
    public void SetInvestment(Investment data)
    {
        this.investment = data;
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (investment != null)
        {
            nameText.text = investment.Name;
            priceText.text = $"Cost: {investment.Cost}€";
            monthlyIncomeText.text = $"Monthly income: {investment.MonthlyPassiveIncome}€";
        }
    }
    public void Buy()
    {
        if (MoneyManager.Instance == null) { onBuyFail?.Invoke(this, MoneyManager.PurchaseFailedReason.MONEYMANAGER_MISSING); }
        if (MoneyManager.Instance.CanAfford(investment.Cost))
        {
            onBuySuccess?.Invoke(this);
        }
        else
        {
            onBuyFail?.Invoke(this, MoneyManager.PurchaseFailedReason.NO_MONEY);
        }
    }

    public Investment GetData()
    {
        return investment;
    }
}
