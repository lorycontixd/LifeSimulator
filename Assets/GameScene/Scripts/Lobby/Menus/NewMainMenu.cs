using Michsky.MUIP;
using Michsky.UI.MTP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lore.Game.Lobby.Menus
{
    public class NewMainMenu : LobbyMenu
    {
        public float startDelay = 3f;
        [SerializeField] private StyleManager startMessage;
        [SerializeField] private NotificationManager notif;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private LastScorePanelLobby lastScorePanel = null;
        [SerializeField] private ButtonManager detailsButton;

        private bool HasSpawnedNotifThisGame = false;

        public override void Start()
        {
            startMessage.playOnEnable = false;
            base.Start();
            ScoreManager.Scores scores = ScoreManager.Instance.lastScores;
            scoreText.gameObject.SetActive(true);
            if (lastScorePanel != null )
            {
                lastScorePanel.Close();
            }
            if (scores != null)
            {
                scoreText.text = $"Last score: {scores.Score}\nHigh score: {ScoreManager.Instance.HighestScore}";
                detailsButton.gameObject.SetActive(true);
            }
            else
            {
                scoreText.text = $"Play games to set your personal score!";
                if (detailsButton != null)
                {
                    detailsButton.gameObject.SetActive(false);
                    if (lastScorePanel != null)
                    {
                        lastScorePanel.CanBeOpened = false;
                        lastScorePanel.Close();
                    }
                }
            }

        }

        public override bool OnOpen()
        {
            startMessage.gameObject.SetActive(false);
            quitPanel.gameObject.SetActive(false);
            return true;
        }

        public override void UpdateUI()
        {
        }

        private IEnumerator StartCo()
        {
            startMessage.Play();
            yield return new WaitForSeconds(startDelay);
            SceneManager.LoadScene(1);
        }

        #region Buttons
        public void ButtonStart()
        {
            if (LobbyFileManager.Instance.GetInfoMenuTimesOpened() < 1  && LobbyFileManager.Instance.GetGameTimesOpened() < 2)
            {
                if (!HasSpawnedNotifThisGame)
                {
                    notif.OpenNotification();
                    HasSpawnedNotifThisGame = true;
                    return;
                }

            }
            startMessage.gameObject.SetActive(true);
            StartCoroutine(StartCo());
        }
        public void ButtonInfo()
        {
            LobbyMenuController.Instance.SwitchMenu(LobbyMenuType.INFO);
            LobbyFileManager.Instance.SaveInfoMenuOpened();
        }
        public void ButtonSettings()
        {

        }
        public void ButtonQuit()
        {
            quitPanel.SetActive(true);
        }
        public void ButtonQuitYes()
        {
            Application.Quit();
        }
        public void ButtonQuitNo()
        {
            quitPanel.SetActive(false);
        }
        public void ButtonDetails()
        {
            lastScorePanel.Open();
        }
        #endregion
    }
}

