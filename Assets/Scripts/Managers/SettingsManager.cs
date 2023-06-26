using ES3Internal;
using ES3Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SettingsManager : MonoBehaviour
{
    #region Singleton
    private static SettingsManager _instance;
    public static SettingsManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    [SerializeField] private List<Settings> settings = new List<Settings>();

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
}
