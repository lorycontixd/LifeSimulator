using System.Collections;
using System.Collections.Generic;
using TMPro;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lore.Game.Lobby.Menus
{
    public enum LobbyMenuType
    {
        MAIN,
        INFO,
        SETTINGS
    }


    public abstract class LobbyMenu : MonoBehaviour
    {
        [Header("Menu Info")]
        [HideInInspector] public int index;
        public LobbyMenuType type;
        public string MenuName;

        [Header("Components")]
        [SerializeField] private List<GameObject> objectsActivateOnOpen = new List<GameObject>();


        public virtual void Start()
        {
            index = (int)type;

            // Listen to database operations
        }

        public virtual bool OnOpen()
        {
            gameObject.SetActive(true);
            foreach (var obj in objectsActivateOnOpen)
            {
                obj.SetActive(true);
            }
            return true;
        }
        public virtual bool OnClose(bool leaveOpen = false)
        {
            gameObject.SetActive(leaveOpen);
            return true;
        }

        public abstract void UpdateUI();
    }
}
