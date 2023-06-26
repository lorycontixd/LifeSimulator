using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class GameManager : BaseManager
    {
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

        [Range(0f, 10f)] public float InitialDelaySeconds = 5f;

        [Header("Simulation Settings")]
        [Range(0f, 5000f), Tooltip("How much money the player gets at the end of the month")] public float MonthlySalary = 500f;

        private GamePlayer player;


        private IEnumerator Start()
        {
            if (player == null)
            {
                player = FindFirstObjectByType<GamePlayer>();
            }
            yield return new WaitForSeconds(InitialDelaySeconds);
            player.Setup();
            IsSetup = true;
        }

        #region Event Listeners
        #endregion
    }
}
