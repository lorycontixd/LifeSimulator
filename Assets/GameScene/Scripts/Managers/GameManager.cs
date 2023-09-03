using Lore.Game.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class GameManager : BaseManager
    {
        #region Enums
        public enum GameState
        {
            STARTING,
            PLAYING,
            FINISH
        }
        #endregion

        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

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

        [Range(0f, 10f)] public float InitialDelaySeconds = 3f;
        [SerializeField] private GameObject debugObjects = null;
        public GameState state { get; private set; }

        [Header("Simulation Settings")]
        [Range(0f, 5000f), Tooltip("How much money the player gets at the end of the month")] public float MonthlySalary = 500f;

        public Action<GameState, GameState> OnGameStateChanged;

        private GamePlayer player;
        private NewPlayerStats playerStats;
        private GameState lastGameState;


        private new IEnumerator Start()
        {
            state = GameState.STARTING;
            SetState(GameState.STARTING);
            if (debugObjects != null)
            {
                debugObjects.SetActive(false);
            }
            if (player == null)
            {
                player = FindFirstObjectByType<GamePlayer>();
            }
            playerStats = player.GetComponent<NewPlayerStats>();
            playerStats.onDeath += OnPlayerDeath;
            yield return new WaitForSeconds(InitialDelaySeconds);
            SetState(GameState.PLAYING);
            IsSetup = true;
        }

        private void OnPlayerDeath(NewPlayerStats.DamageReason reason)
        {
            SetState(GameState.FINISH);
            ScoreManager.Instance.SetScores(new ScoreManager.Scores(true));
            ScoreManager.Instance.SaveScores();
        }

        public void SetState(GameState newState)
        {
            lastGameState = state;
            state = newState;
            OnGameStateChanged?.Invoke(lastGameState, state);
        }

        #region Event Listeners
        #endregion
    }
}
