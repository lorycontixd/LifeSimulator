using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelItemPrefab;
    [SerializeField] private Transform itemHolder;

    [Header("UI")]
    [SerializeField] private ProgressBar totalFriendshipBar;

    public Action onPanelClose;

    private List<FriendsPanelItem> clones = new List<FriendsPanelItem>();


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => FriendshipManager.Instance.IsReady); ;
        totalFriendshipBar.maxValue = FriendshipManager.Instance.MaxFriendship;   
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            totalFriendshipBar.ChangeValue(FriendshipManager.Instance.AverageFriendship);
            UpdateItems();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onPanelClose?.Invoke();
    }
    public void Open()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void UpdateUI()
    {
        ClearItems();
        SpawnItems();
    }
    public void UpdateItems()
    {
        for (int i=0; i<clones.Count; i++)
        {
            clones[i].UpdateUI();
        }
    }
    private void ClearItems()
    {
        clones.Clear();
        for (int i=0; i<itemHolder.childCount; i++)
        {
            Destroy(itemHolder.GetChild(i).gameObject);
        }
    }
    private void SpawnItems()
    {
        for (int i=0; i<FriendshipManager.Instance.friends.Count; i++)
        {
            Friendship friendship = FriendshipManager.Instance.friends[i];
            GameObject clone = Instantiate(panelItemPrefab, itemHolder);
            FriendsPanelItem item = clone.GetComponentInParent<FriendsPanelItem>();
            if (item != null)
            {
                item.SetFriendship(friendship);
                clones.Add(item);
            }
            else
            {
                Destroy(clone);
            }
        }
    }
}
