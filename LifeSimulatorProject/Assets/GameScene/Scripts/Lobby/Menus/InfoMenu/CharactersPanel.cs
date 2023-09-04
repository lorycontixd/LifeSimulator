using Lore.Game.Lobby.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersPanel : MonoBehaviour
{
    #region CharacterWindow
    public enum Character
    {
        PLAYER,
        DOG,
        CITIZEN,
        POLICEMAN,
        NURSE,
        RECEPTIONIST
    }

    [Serializable]
    public struct CharacterWindow
    {
        public Character character;
        public GameObject window;
    }
    #endregion

    [Header("Windows")]
    [SerializeField] private GameObject mainWindow;
    [SerializeField] private GameObject windowsHolder;
    public List<CharacterWindow> windows = new List<CharacterWindow>();

    private LobbyMenu parentMenu;


    public void Open(LobbyMenu parentMenu)
    {
        gameObject.SetActive(true);
        CloseAllCharacterWindows();
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
        windowsHolder.SetActive(false);
        if (mainWindow != null)
        {
            mainWindow.SetActive(false);
        }
    }

    public void CloseAllWindows()
    {
        mainWindow.gameObject.SetActive(false);
        CloseAllCharacterWindows();
    }
    public void CloseAllCharacterWindows()
    {
        foreach(var characterWindow in windows)
        {
            characterWindow.window.SetActive(false);
        }
    }

    public void ButtonPlayer()
    {
        OpenWindow(Character.PLAYER);
    }
    public void ButtonDog()
    {
        OpenWindow(Character.DOG);
    }
    public void ButtonPoliceman()
    {
        OpenWindow(Character.POLICEMAN);
    }
    public void ButtonCitizen()
    {
        OpenWindow(Character.CITIZEN);
    }
    public void ButtonNurse()
    {
        OpenWindow(Character.NURSE);
    }
    public void ButtonReceptionist()
    {
        //OpenWindow(Character.RECEPTIONIST);
    }

    public void OpenWindow(Character c)
    {
        CloseAllWindows();
        CharacterWindow w = windows.FirstOrDefault(w => w.character == c);
        w.window.SetActive(true);
    }

    public void WindowToMain()
    {
        CloseAllCharacterWindows();
        mainWindow.SetActive(true);
    }

    public void ButtonBack()
    {
        if (parentMenu != null)
        {
            Close();
            parentMenu.OnOpen();
        }
    }
}
