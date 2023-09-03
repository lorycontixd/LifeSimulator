using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    #region Volume Increase
    public enum VolumeIncreaseMode
    {
        LINEAR,
        QUADRATIC,
        QUARTIC
    }
    #endregion

    #region Setting Window
    private enum SettingWindow
    {
        SIMULATION,
        AUDIO,
        GRAPHICS
    }
    #endregion

    [Header("Components")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Setting Windows")]
    [SerializeField] private GameObject simulationWindow;
    [SerializeField] private GameObject audioWindow;
    [SerializeField] private GameObject graphicsWindow;

    [Header("Simulation Components")]
    [SerializeField] private Slider dayDuration;
    [SerializeField] private Slider simulationSpeed;
    [SerializeField] private Toggle includeBirds;
    [SerializeField] private CustomDropdown playerPersonality;
    [SerializeField] private Slider playerSpeed;

    [Header("Audio Components")]
    [SerializeField] private Slider masterVolume;
    [SerializeField] private CustomDropdown ingameMusic;

    [Header("Save Settings")]
    [SerializeField] private string settingsDirectory = "settings";
    [SerializeField] private string simulationSettingsFile = "simulation_settings_savefile.es3";
    [SerializeField] private string audioSettingsFile = "audio_settings_savefile.es3";
    [SerializeField] private string graphicsSettingsFile = "graphics_settings_savefile.es3";

    [Header("General settings")]
    [SerializeField] private bool debugMode;
    [SerializeField] private VolumeIncreaseMode volumeIncreaseMode;

    private string saveFilePassword = "Lorenzo123";
    private string settingsDirectoryPath;
    private string simulationSettingsPath;
    private string audioSettingsPath;
    private string graphicsSettingsPath;
    private ES3Settings simulationSettings;
    private ES3Settings audioSettings;
    private ES3Settings graphicsSettings;
    private SettingWindow currentWindow = SettingWindow.SIMULATION;

    private void Start()
    {
        ES3.Init();


        settingsDirectoryPath = Path.Combine(Application.persistentDataPath, settingsDirectory);
        if (!Directory.Exists(settingsDirectoryPath))
            Directory.CreateDirectory(settingsDirectoryPath);

        simulationSettingsPath = Path.Combine(Application.persistentDataPath, "settings", simulationSettingsFile);
        audioSettingsPath = Path.Combine(Application.persistentDataPath, "settings", audioSettingsFile);
        graphicsSettingsPath = Path.Combine(Application.persistentDataPath, "settings", graphicsSettingsFile);
        simulationSettings = new ES3Settings(simulationSettingsPath, ES3.EncryptionType.AES);
        audioSettings = new ES3Settings(audioSettingsPath, ES3.EncryptionType.AES);
        graphicsSettings = new ES3Settings(graphicsSettingsPath, ES3.EncryptionType.AES);


        if (!File.Exists(simulationSettingsPath))
        {
            SimulationSettingsSave();
        }
        if (!File.Exists(audioSettingsPath))
        {
            AudioSettingsSave();
        }
        if (!File.Exists(graphicsSettingsPath))
        {
            GraphicSettingsSave();
        }

        SimulationSettingsLoad();
        AudioSettingsLoad();
        GraphicSettingsLoad();
        SetVolume(masterVolume.value);

        ButtonSimulation();
    }

    #region Windows
    public void CloseAllWindows()
    {
        simulationWindow.SetActive(false);
        audioWindow.SetActive(false);
        graphicsWindow.SetActive(false);
    }
    public void ButtonSimulation()
    {
        CloseAllWindows();
        simulationWindow.SetActive(true);
    }
    public void ButtonAudio()
    {
        CloseAllWindows();
        audioWindow.SetActive(true);
    }
    public void ButtonGraphics()
    {
        CloseAllWindows();
        graphicsWindow.SetActive(true);

        GraphicSettingsLoad();
    }
    #endregion

    #region Audio Settings
    public void SetVolume(float volume)
    {
        float NewValue = 0f;
        if (volumeIncreaseMode == VolumeIncreaseMode.LINEAR)
        {
            NewValue = volume - 80f; // Linear
        }else if (volumeIncreaseMode == VolumeIncreaseMode.QUADRATIC)
        {
            NewValue = -Mathf.Pow((volume - 100), 2f) / Mathf.Pow(10f, 2f) + 20f; // Quadratic
        }else if (volumeIncreaseMode == VolumeIncreaseMode.QUARTIC)
        {
            NewValue = -Mathf.Pow((volume - 100), 4f) / Mathf.Pow(10f, 6f) + 20f; // Quadratic
        }
        masterMixer.SetFloat("Volume", NewValue);
    }
    #endregion

    #region Settings Save & Load
    public void SimulationSettingsSave()
    {
        ES3.Save("DayDuration", dayDuration.value, simulationSettingsPath);
        ES3.Save("SimulationSpeed", simulationSpeed.value, simulationSettingsPath);
        ES3.Save("IncludeBirds", includeBirds.isOn, simulationSettingsPath);
        ES3.Save("PlayerPersonality", playerPersonality.selectedItemIndex, simulationSettingsPath);
        ES3.Save("PlayerSpeed", playerSpeed.value, simulationSettingsPath);
    }
    public void SimulationSettingsLoad()
    {
        float _dayDuration = ES3.Load<float>("DayDuration", simulationSettingsPath);
        float _simulationSpeed = ES3.Load<float>("SimulationSpeed", simulationSettingsPath);
        bool _includeBids = ES3.Load<bool>("IncludeBirds", simulationSettingsPath);
        int _playerPersonality = ES3.Load<int>("PlayerPersonality", simulationSettingsPath);
        float _playerSpeed = ES3.Load<float>("PlayerSpeed", simulationSettingsPath);

        dayDuration.value = _dayDuration;
        simulationSpeed.value = _simulationSpeed;
        includeBirds.isOn = _includeBids;
        playerPersonality.selectedItemIndex = _playerPersonality;
        playerPersonality.UpdateItemLayout();
        playerSpeed.value = _playerSpeed;
    }
    public void AudioSettingsSave()
    {
        ES3.Save("MasterVolume", masterVolume.value, audioSettingsPath);
        ES3.Save("IngameMusic", ingameMusic.selectedItemIndex, audioSettingsPath);
    }
    public void AudioSettingsLoad()
    {
        float _masterVolume = ES3.Load<float>("MasterVolume", audioSettingsPath);
        int _ingameMusic = ES3.Load<int>("IngameMusic", audioSettingsPath);

        masterVolume.value = _masterVolume;
        ingameMusic.selectedItemIndex = _ingameMusic;
    }
    public void GraphicSettingsSave()
    {

    }
    public void GraphicSettingsLoad()
    {

    }
    #endregion
}
