using DG.Tweening;
using Lore.Game.Managers;
using Lore.Game.UI;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCommandsPanel : MonoBehaviour
{
    [SerializeField] private GameObject commandsPanel;
    [SerializeField] private BuildingsConstructionPanel buildingConstructionPanel;
    [SerializeField] private ButtonManager closeCommandsButton;
    [SerializeField] private ButtonManager openCommandsButton;

    private float defaultBuildingCommandsPanelX;
    private float defaultBuildingButtonX;
    private float buildingCommandsPanelTranslateX = 225;



    private void Start()
    {
        defaultBuildingCommandsPanelX = commandsPanel.transform.position.x;
        defaultBuildingButtonX = openCommandsButton.transform.position.x;
        commandsPanel.transform.Translate(defaultBuildingCommandsPanelX + buildingCommandsPanelTranslateX, 0f, 0f);
        openCommandsButton.transform.Translate(defaultBuildingButtonX + buildingCommandsPanelTranslateX, 0f, 0f);
        closeCommandsButton.transform.Translate(defaultBuildingButtonX + buildingCommandsPanelTranslateX, 0f, 0f);
        CloseCommandsPanel();
    }

    public void CloseCommandsPanel()
    {
        closeCommandsButton.gameObject.SetActive(false);
        openCommandsButton.gameObject.SetActive(true);
        openCommandsButton.transform.DOMoveX(defaultBuildingButtonX + buildingCommandsPanelTranslateX, 2f);
        closeCommandsButton.transform.DOMoveX(defaultBuildingButtonX + buildingCommandsPanelTranslateX, 2f);
        commandsPanel.transform.DOMoveX(defaultBuildingCommandsPanelX + buildingCommandsPanelTranslateX, 2f);
        buildingConstructionPanel.Close();
    }
    public void OpenCommandsPanel()
    {
        closeCommandsButton.gameObject.SetActive(true);
        openCommandsButton.gameObject.SetActive(false);
        Debug.Log($"Opening commands panel");
        openCommandsButton.transform.DOMoveX(defaultBuildingButtonX, 2f);
        closeCommandsButton.transform.DOMoveX(defaultBuildingButtonX, 2f);
        commandsPanel.transform.DOMoveX(defaultBuildingCommandsPanelX , 2f);
    }

    #region Building Commands
    public void ButtonBuild()
    {
        buildingConstructionPanel.Open();
        BuildingManager.Instance.EnterConstructionMode();
    }
    public void ButtonSell()
    {

    }
    #endregion
}
