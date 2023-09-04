using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lore.Game.Lobby.Menus
{
    public class NewInfoMenu : LobbyMenu
    {
        [Header("UI Components")]
        [SerializeField] private GameObject mainWindow;
        [SerializeField] private Button gameInfoButton;
        [SerializeField] private Button charactersButton;

        [SerializeField] private CharactersPanel charactersPanel;
        [SerializeField] private GameInfoPanel gameInfoPanel;

        private bool charactersButtonScaled = false;
        private bool gameinfoButtonScaled = false;

        public override bool OnOpen()
        {
            if (charactersPanel == null || gameInfoPanel == null)
            {
                return false;
            }
            mainWindow.SetActive(true);
            charactersPanel.Close();
            gameInfoPanel.Close();
            return base.OnOpen();
        }


        public override void UpdateUI()
        {
        }

        #region Buttons
        public void ButtonBack()
        {
            LobbyMenuController.Instance.SwitchMenu(LobbyMenuType.MAIN);
        }
        public void ButtonGameInfo()
        {
            mainWindow.SetActive(false);
            gameInfoPanel.Open(this);
        }
        public void ButtonCharacters()
        {
            mainWindow.SetActive(false);
            charactersPanel.Open(this);
        }

        public void ButtonCharactersPointerEnter(BaseEventData data)
        {
            if (!charactersButtonScaled)
            {
                charactersButton.transform.DOShakeScale(2f, 0.1f, 3, 45);
            }
        }
        public void ButtonCharactersPointerExit(BaseEventData data)
        {
            if (charactersButtonScaled)
            {
                charactersButton.transform.DOBlendableScaleBy(new Vector3(-0.2f, -0.2f, -0.2f), 2f);
                charactersButtonScaled = false;
            }
        }
        #endregion
    }
}
