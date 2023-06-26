using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : MonoBehaviour
{
    [SerializeField] private Text stateText;


    void LateUpdate()
    {
        Dictionary<string, object> worldStates = GWorld.Instance.GetWorld().GetStates();
        stateText.text = "";

        foreach(KeyValuePair<string, object> kvp in worldStates)
        {
            stateText.text += kvp.Key + ", " + kvp.Value + "\n";
        }
    }
}
