using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class PlayerHouseRegion : MonoBehaviour
{
    public bool IsPlayerInHouse { get; private set; }
    private GamePlayer player;
    private BoxCollider boxCollider;

    public UnityEvent onPlayerEnterHouse;
    public UnityEvent onPlayerExitHouse;


    private void Start()
    {
        player = FindFirstObjectByType<GamePlayer>();
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError($"Player house region required box collider attached");
            return;
        }
        boxCollider.isTrigger = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsPlayerInHouse = true;
            player.beliefs.ModifyState("IsHome", true);
            onPlayerEnterHouse?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsPlayerInHouse = false;
            player.beliefs.ModifyState("IsHome", false);
            onPlayerExitHouse?.Invoke();
        }
    }
}
