using DG.Tweening;
using Lore.Game.Managers;
using Lore.Game.UI;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsPanel : MonoBehaviour
{
    public float commandsPanelTranslateX = 310f;
    [SerializeField] private GameObject commandsPanel;
    [SerializeField] private BuildingsConstructionPanel buildingConstructionPanel;
    [SerializeField] private InvestmentsPanel investmentsPanel;
    [SerializeField] private FriendsPanel friendsPanel;

    [SerializeField] private ButtonManager closeCommandsButton;
    [SerializeField] private ButtonManager openCommandsButton;

    private float defaultCommandsPanelX;
    private float defaultButtonX;
    private GameObject activePanel = null;
    private bool sellWarningSpawned = false;


    private void Start()
    {
        defaultCommandsPanelX = commandsPanel.transform.position.x;
        defaultButtonX = openCommandsButton.transform.position.x;
        buildingConstructionPanel.onPanelClose += OnPanelClose;
        investmentsPanel.onPanelClose += OnPanelClose;
        friendsPanel.onPanelClose += OnPanelClose;
        //CloseCommandsPanel();
        closeCommandsButton.gameObject.SetActive(true);
        openCommandsButton.gameObject.SetActive(false);
    }

    private void OnPanelClose()
    {
        activePanel = null;
    }

    public void CloseCommandsPanel()
    {
        closeCommandsButton.gameObject.SetActive(false);
        openCommandsButton.gameObject.SetActive(true);
        openCommandsButton.transform.DOMoveX(defaultButtonX + commandsPanelTranslateX, 2f);
        closeCommandsButton.transform.DOMoveX(defaultButtonX + commandsPanelTranslateX, 2f);
        commandsPanel.transform.DOMoveX(defaultCommandsPanelX + commandsPanelTranslateX, 2f);
        buildingConstructionPanel.Close();
        investmentsPanel.Close();
        friendsPanel.Close();
    }
    public void OpenCommandsPanel()
    {
        closeCommandsButton.gameObject.SetActive(true);
        openCommandsButton.gameObject.SetActive(false);
        openCommandsButton.transform.DOMoveX(defaultButtonX, 2f);
        closeCommandsButton.transform.DOMoveX(defaultButtonX, 2f);
        commandsPanel.transform.DOMoveX(defaultCommandsPanelX , 2f);
    }

    #region Commands
    public void ButtonBuild()
    {
        Debug.Log($"Active panel");
        if (activePanel != null) return;
        bool success = BuildingManager.Instance.EnterConstructionMode();
        if (!success)
        {
            Lore.Game.Managers.NotificationManager.Instance.Warning("Invalid command", "Cannot enter build mode while player is performing an action. Please wait.");
            return;
        }
        buildingConstructionPanel.Open();
        activePanel = buildingConstructionPanel.gameObject;
    }
    public void ButtonSell()
    {
        if (!sellWarningSpawned)
        {
            Lore.Game.Managers.NotificationManager.Instance.Warning("Not implemented", "Sorry, selling buildings is not implemented at the moment.");
            sellWarningSpawned = true;
        }
        return;
    }
    private IEnumerator SellWarningCooldown()
    {
        yield return new WaitForSeconds(12f);
        sellWarningSpawned = false;
    }
    public void ButtonInvest()
    {
        if (activePanel != null) return;
        investmentsPanel.Open();
        InvestManager.Instance.EnterInvestmentMode();
        activePanel = buildingConstructionPanel.gameObject;
    }
    public void ButtonFriends()
    {
        if (activePanel != null) return;
        friendsPanel.Open();
        activePanel = friendsPanel.gameObject;
    }
    #endregion
}
