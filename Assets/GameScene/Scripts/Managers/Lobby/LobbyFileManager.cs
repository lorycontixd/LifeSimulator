using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LobbyFileManager : MonoBehaviour
{
    #region Singleton
    private static LobbyFileManager _instance;
    public static LobbyFileManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
    #endregion


    public string SaveFilename = "lobby.es3";
    private string SavePath;
    private ES3Settings settings;

    private int timesGameOpened = 0;
    private int timesInfoMenuOpened = 0;

    private void Start()
    {
        SavePath = Path.Combine(Application.persistentDataPath, SaveFilename);
        settings = new ES3Settings(SavePath, ES3.EncryptionType.None);
        LoadGameTimesOpened();
        LoadInfoMenuTimesOpened();
        SaveGameOpened();
    }

    private bool IsFirstTime()
    {
        return !ES3.FileExists(settings);
    }
    public void LoadGameTimesOpened()
    {
        if (IsFirstTime()) { timesInfoMenuOpened = 0; return; }
        timesGameOpened = ES3.Load<int>("GamesOpenedTimes", settings);
    }
    public void LoadInfoMenuTimesOpened()
    {
        if (IsFirstTime()) { timesInfoMenuOpened = 0; return; }
        if (!ES3.KeyExists("InfoMenuOpenedTimes")) { timesInfoMenuOpened = 0; return; }
        timesInfoMenuOpened = ES3.Load<int>("InfoMenuOpenedTimes", settings);
    }
    public void SaveGameOpened()
    {
        ES3.Save<int>("GamesOpenedTimes", timesGameOpened + 1, settings);
    }
    public void SaveInfoMenuOpened()
    {
        ES3.Save<int>("InfoMenuOpenedTimes", timesInfoMenuOpened + 1, settings);
    }
    public int GetInfoMenuTimesOpened()
    {
        return timesInfoMenuOpened;
    }
    public int GetGameTimesOpened()
    {
        return timesGameOpened;
    }

}
