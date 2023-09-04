using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestFriendshipUIManager : MonoBehaviour
{
    [SerializeField] private GameObject interactButtonPrefab;
    [SerializeField] private Transform holder;
    [SerializeField] private TextMeshProUGUI timeText;


    private void Start()
    {
        if (FriendshipManager.Instance != null)
        {
            FriendshipManager.Instance.onFriendshipAdd += OnFriendshipAdd;
        }
    }
    private void Update()
    {
        if (timeText != null)
        {
            timeText.text = $"Time: {Time.realtimeSinceStartup.ToString("#.##")}";
        }
    }

    private void OnFriendshipAdd(Friendship friendship)
    {
        ClearItems();
        SpawnItems();
    }


    private void ClearItems()
    {
        for (int i=0; i<holder.childCount; i++)
        {
            Destroy(holder.GetChild(i).gameObject);
        }
    }
    private void SpawnItems()
    {
        for (int i=0; i<FriendshipManager.Instance.friends.Count; i++)
        {
            Friendship friend = FriendshipManager.Instance.friends[i];
            var clone = Instantiate(interactButtonPrefab, holder);
            clone.name = $"Friend {friend.ID}";
            clone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Interact with F {friend.ID}";
            TestFriendshipHolder h = clone.GetComponent<TestFriendshipHolder>();
            h.SetFriendship(friend);
            h.onButtonPress += OnInteractFriend;
        }
    }

    private void OnInteractFriend(Friendship friendship)
    {
        friendship.Interact();
    }
}
