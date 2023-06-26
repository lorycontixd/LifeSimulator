using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

namespace TestScene
{
    public class PlayerStatsCanvas : MonoBehaviour
    {
        Player player;
        [SerializeField] private GameObject stateTextPrefab;

        private void Start()
        {
            player = GameObject.FindFirstObjectByType<Player>();
        }

        private void Update()
        {
            WriteStats();
        }

        void WriteStats()
        {
            if (stateTextPrefab == null)
            {
                Debug.LogWarning($"[PlayerStatsCanvas] State text prefab is missing. Skipping player stats display.");
                return;
            }
            ClearStats();
            foreach (KeyValuePair<string, object> kvp in player.beliefs.states)
            {
                if (kvp.Value != null)
                {
                    SpawnStat(kvp);
                }
            }
        }

        void SpawnStat(KeyValuePair<string, object> kvp)
        {
            GameObject clone = Instantiate(stateTextPrefab, this.transform);
            TextMeshProUGUI text = clone.GetComponent<TextMeshProUGUI>();
            text.text = $"{kvp.Key} - {kvp.Value}";
        }

        void ClearStats()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

}
