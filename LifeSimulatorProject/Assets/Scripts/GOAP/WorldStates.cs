using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOAP.GWorld;

[System.Serializable]
public class WorldState {

    public string key;
    public int value;
}

public class WorldStates {
    #region WorldStateChangeType
    public enum WorldStateChangeType
    {
        ADD,
        MODIFY,
        REMOVE
    }
    #endregion

    public Dictionary<string, object> states;
    public Action<WorldStateChangeType, string> onWorldStateChange;

    public WorldStates() {

        states = new Dictionary<string, object>();
    }

    public bool HasState(string key) => states.ContainsKey(key);

    public void AddState(string key, object value) {

        states.Add(key, value);
        onWorldStateChange?.Invoke(WorldStateChangeType.ADD, key);
    }

    public void RemoveState(string key) {

        if (HasState(key))
        {
            onWorldStateChange?.Invoke(WorldStateChangeType.REMOVE, key);
            states.Remove(key);
        }
    }

    public void ModifyState(string key, object value) {

        if (HasState(key)) {
            states[key] = value;
            onWorldStateChange?.Invoke(WorldStateChangeType.MODIFY, key);
        } else {

            AddState(key, value);
        }
    }


    public Dictionary<string, object> GetStates() => states;
}