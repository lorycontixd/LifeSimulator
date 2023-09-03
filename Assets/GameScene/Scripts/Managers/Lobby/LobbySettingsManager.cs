using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySettingsManager : MonoBehaviour
{
    #region Utilities
    public class Settings
    {

    }

    public static float PercentageToVolume(float perc)
    {
        float newVal = (((perc - 0) * (20 + 80)) / (100 - 0)) - 80;
        return newVal;
    }
    #endregion



}
