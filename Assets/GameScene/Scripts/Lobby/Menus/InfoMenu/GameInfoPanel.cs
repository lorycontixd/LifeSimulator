using Lore.Game.Lobby.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInfoPanel : MonoBehaviour
{
    #region Utilities
    public enum GameinfoWindowType
    {
        STATS,
        DISEASES,
        BUILDINGS,
        INVESTMENTS
    }
    [Serializable]
    public struct GameinfoWindow
    {
        public GameinfoWindowType windowType;
        public GameObject window;
    }
    #endregion

    public bool IsOpen { get; protected set; }

    [SerializeField] private GameObject windowsHolder;
    [SerializeField] private List<GameinfoWindow> windows = new List<GameinfoWindow>();
    [SerializeField] private GameObject mainWindow;

    private LobbyMenu parentMenu;


    public void Open(LobbyMenu parentMenu)
    {
        gameObject.SetActive(true);
        CloseAllInfoWindows();
        windowsHolder.SetActive(true);
        if (mainWindow != null)
        {
            mainWindow.SetActive(true);
        }
        this.parentMenu = parentMenu;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void CloseAllWindows()
    {
        CloseAllInfoWindows();
        mainWindow.SetActive(false);
    }
    public void CloseAllInfoWindows()
    {
        foreach(var window in windows)
        {
            window.window.SetActive(false);
        }
    }

    public void OpenWindow(GameinfoWindowType type)
    {
        CloseAllWindows();
        GameinfoWindow w = windows.FirstOrDefault(x => x.windowType == type);
        w.window.SetActive(true);
    }
    public void WindowToMain()
    {
        CloseAllInfoWindows();
        mainWindow.SetActive(true);
    }

    #region Buttons
    public void ButtonStats()
    {
        OpenWindow(GameinfoWindowType.STATS);
    }
    public void ButtonBuildings()
    {
        OpenWindow(GameinfoWindowType.BUILDINGS);
    }
    public void ButtonDiseases()
    {
        OpenWindow(GameinfoWindowType.DISEASES);
    }
    public void ButtonInvestments()
    {
        OpenWindow(GameinfoWindowType.INVESTMENTS);
    }
    public void ButtonBack()
    {
        if (parentMenu != null)
        {
            Close();
            parentMenu.OnOpen();
        }
    }
    #endregion

}
