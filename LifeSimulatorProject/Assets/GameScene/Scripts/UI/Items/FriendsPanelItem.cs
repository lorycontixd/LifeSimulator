using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsPanelItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image image;
    [SerializeField] private ProgressBar friendshipBar;

    private Friendship friendship;


    private void Start()
    {
        if (image == null) { image = GetComponent<Image>(); }
        if (friendshipBar == null) {  friendshipBar = GetComponent<ProgressBar>(); }

        this.friendshipBar.maxValue = (FriendshipManager.Instance != null) ? FriendshipManager.Instance.MaxFriendship : 100f;
    }

    public void SetFriendship(Friendship friendship)
    {
        this.friendship = friendship;
    }

    public void UpdateUI()
    {
        this.friendshipBar.ChangeValue(this.friendship.CurrentFriendship);
    }

}
