using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendshipManager : MonoBehaviour
{
    #region Singleton
    private static FriendshipManager _instance;
    public static FriendshipManager Instance { get { return _instance; } }

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


    public bool AutoAssignID = true;
    public bool AutoAssignProps = false;
    public Vector2 FriendshipDecayRange = new Vector2(3f, 7f);
    public Vector2 MoodDecayRange = new Vector2(5f, 11f);
    public Vector2 MoodDecayDelay = new Vector2(30f, 80f);
    public List<Friendship> friends { get; private set; } = new List<Friendship>();
    public float AverageFriendship { get => friends.Average(f => f.CurrentFriendship); }


    [Header("Settings")]
    public float MaxFriendship = 100f;
    public float MaxMood = 100f;
    [SerializeField] private bool DebugText = false;

    public bool IsReady { get; private set; } = false;
    public Action<Friendship> onFriendshipAdd;


    private void Start()
    {
        IsReady = true;
    }
    private void Update()
    {;
        if (DebugText)
            Debug.Log($"Avg friendship: {AverageFriendship}");
    }

    public void AddFriend(Friendship friend)
    {
        if (!friends.Contains(friend))
        {
            friends.Add(friend);
            onFriendshipAdd?.Invoke(friend);
            if (AutoAssignID)
            {
                friend.ID = friends.Count;
            }
            if (AutoAssignProps)
            {
                friend.MoodDecay = UnityEngine.Random.Range(MoodDecayRange.x, MoodDecayRange.y);
                friend.FriendshipDecay = UnityEngine.Random.Range(FriendshipDecayRange.x, FriendshipDecayRange.y);
                friend.TimeForMoodDecay = UnityEngine.Random.Range(MoodDecayDelay.x, MoodDecayDelay.y) ;
            }
        }
    }

    public float CalculateAverageFriendship()
    {
        return friends.Average(f => f.CurrentFriendship);
    }

    public int CountMoodyFriends()
    {
        return friends.Count(f => !f.MoodIsMax);
    }
    public int CountLosingFriends()
    {
        return friends.Count(f => !f.FriendshipIsMax);
    }
}
