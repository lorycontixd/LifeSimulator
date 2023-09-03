using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendship : MonoBehaviour
{
    public enum FriendshipInitMode
    {
        RANDOM,
        FULL,
        HALF
    }

    public int ID;
    public FriendshipInitMode friendshipInitMode;
    public float CurrentFriendship { get; private set; }
    public float FriendshipDecay = 11f;
    public float CurrentMood { get; private set; }
    public float MoodDecay = 8f;
    public float TimeForMoodDecay = 45f;
    public bool MoodIsMax { get => CurrentMood >= FriendshipManager.Instance.MaxMood; }
    public bool FriendshipIsMax { get => CurrentFriendship >= FriendshipManager.Instance.MaxFriendship; }

    private float _lastInteractedGametime;
    private bool _isDecayingMood;
    private bool _isDecayingFriendship;

    [Header("Settings")]
    [SerializeField] private bool RegisterOnStart = true;
    [SerializeField] private bool DebugText;

    private void Start()
    {
        if (FriendshipManager.Instance != null)
        {
            if (RegisterOnStart)
                FriendshipManager.Instance.AddFriend(this);
        }
        SetInitialFriendship();
        CurrentMood = FriendshipManager.Instance.MaxMood;
    }
    private void Update()
    {
        //Debug.Log($"[F{ID} => {Time.realtimeSinceStartup - _lastInteractedGametime}, {TimeForMoodDecay}");
        if (TimeManager.Instance.TimeSinceStart - _lastInteractedGametime > TimeForMoodDecay && !_isDecayingMood)
        {
            _isDecayingMood = true;
        }
        if (_isDecayingMood && CurrentMood > 0)
        {
            CurrentMood = Mathf.Clamp(CurrentMood - MoodDecay * Time.deltaTime, 0f, FriendshipManager.Instance.MaxMood);
            if (DebugText)
                Debug.Log($"[F{ID}] Decaying mood: {CurrentMood}");
            if (CurrentMood <= 0f)
            {
                _isDecayingMood = false;
                _isDecayingFriendship = true;
            }
        }
        if (_isDecayingFriendship && CurrentFriendship > 0)
        {
            CurrentFriendship = Mathf.Clamp(CurrentFriendship - FriendshipDecay * Time.deltaTime, 0f, FriendshipManager.Instance.MaxFriendship);
            if (DebugText)
                Debug.Log($"[F{ID}] Decaying Friendship: {CurrentFriendship}");
            if (CurrentFriendship <= 0f)
            {
                _isDecayingFriendship = false;
            }
        }
    }


    private void SetInitialFriendship()
    {
        switch (friendshipInitMode)
        {
            case FriendshipInitMode.RANDOM:
                CurrentFriendship = UnityEngine.Random.Range(0f, FriendshipManager.Instance.MaxFriendship);
                break;
            case FriendshipInitMode.HALF:
                CurrentFriendship = FriendshipManager.Instance.MaxFriendship / 2;
                break;
            case FriendshipInitMode.FULL:
                CurrentFriendship = FriendshipManager.Instance.MaxFriendship;
                break;
        }
    }

    public void SetNewValue(float newValue)
    {
        CurrentFriendship = newValue;
    }

    public void Interact()
    {
        _isDecayingMood = false;
        _isDecayingFriendship = false;
        _lastInteractedGametime = TimeManager.Instance.TimeSinceStart;
        CurrentFriendship = Mathf.Clamp(CurrentFriendship + 50f, 0f, FriendshipManager.Instance.MaxFriendship);
        CurrentMood = FriendshipManager.Instance.MaxMood;
    }
}
