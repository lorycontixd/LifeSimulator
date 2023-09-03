using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Lore.Game.Lobby.Menus
{
    public class LobbyMenuController : MonoBehaviour
    {
        #region Singleton
        private static LobbyMenuController _instance;
        public static LobbyMenuController Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion


        public List<LobbyMenu> menus = new List<LobbyMenu>();
        public LobbyMenu activeMenu = null;


        private void Start()
        {
            if (menus.Count <= 0)
            {
                Debug.LogError($"No lobby menus were set in the controller.");
                return;
            }
            (bool, string) result = CheckMenusValidity();
            if (!result.Item1)
            {
                // Menus not valid --> Print error message
                Debug.LogError(result.Item2);
                return;
            }

            OpenAllParents();
            CloseAllMenus();
            activeMenu = GetMenuByType(LobbyMenuType.MAIN);
            activeMenu.gameObject.SetActive(true);
            activeMenu.OnOpen();

        }

        /// <summary>
        /// Activates all menu parent objects.
        /// This is because the parent objects must be activated in order to control the components.
        /// </summary>
        public void OpenAllParents()
        {
            foreach (LobbyMenu menu in menus)
            {
                menu.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Close all menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (LobbyMenu menu in menus)
            {
                menu.gameObject?.SetActive(false);
                menu.OnClose();
            }
        }


        /// <summary>
        /// Check if set menus are all valid:
        /// <br> - No menu types are repeated</br>
        /// <br> - No menu indices are repeated</br>
        /// <br> - Mandatory methods are implemented</br>
        /// </summary>
        /// <returns>(bool, string) tuple, where item 1 is validity state and item 2 is the error message in case of a fail.</returns>
        private (bool, string) CheckMenusValidity()
        {
            foreach (LobbyMenuType type in Enum.GetValues(typeof(LobbyMenuType)))
            {
                List<LobbyMenu> menusOfType = menus.Where(m => m.type == type).ToList();
                if (menusOfType.Count > 1)
                {
                    // 2 menus share the same type
                    return (false, $"Two menus share the type {type.ToString()}");
                }
            }
            return (true, "");
        }


        /// <summary>
        /// Get a menu instance by its type.
        /// </summary>
        /// <param name="type">The type of the queried menu</param>
        /// <returns>The instance of the searched menu.</returns>
        public LobbyMenu GetMenuByType(LobbyMenuType type)
        {
            return menus.FirstOrDefault(m => m.type == type);
        }


        /// <summary>
        /// Switch the active menu.
        /// <br>Performs closing operation on active menu and opening operations on the new menu.</br>
        /// </summary>
        /// <param name="newMenuType">The new menu to go to.</param>
        /// <returns>The operation success state</returns>
        public bool SwitchMenu(LobbyMenuType newMenuType)
        {
            LobbyMenu lastmenu = activeMenu;
            LobbyMenu newMenu = GetMenuByType(newMenuType);
            if (!menus.Contains(newMenu))
            {
                return false;
            }
            /*if (newMenuType == LobbyMenuType.QUIT)
            {
                QuitMenu quitMenu = (QuitMenu)newMenu;
                quitMenu.SetLastMenu(activeMenu);
            }*/
            if (activeMenu.OnClose())
            {
                activeMenu.gameObject.SetActive(false);
                activeMenu = newMenu;
                activeMenu.gameObject.SetActive(true);
                if (!activeMenu.OnOpen())
                {
                    lastmenu.OnOpen();
                    activeMenu = lastmenu;
                }
            }
            
            return true;
        }
    }

}
