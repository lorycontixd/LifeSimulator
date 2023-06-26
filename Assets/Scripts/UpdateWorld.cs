using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GOAP;

public class UpdateWorld : MonoBehaviour {

    public Text states;

    private void LateUpdate() {

        Dictionary<string, object> worldStates = GWorld.Instance.GetWorld().GetStates();
        states.text = "";

        foreach (KeyValuePair<string, object> s in worldStates) {

            states.text += s.Key + ", " + s.Value + "\n";
        }
    }
}