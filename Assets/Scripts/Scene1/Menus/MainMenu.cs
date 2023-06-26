using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image logo;

    [Header("Menus")]
    public GameObject homeMenu;
    public GameObject settingsMenu;

    [Header("Music")]
    [SerializeField] private bool PlayMusicOnStart = true;
    [SerializeField] private AudioClip startMusic;

    [Header("Settings")]
    [SerializeField] private bool _animateLogoColours = true;
    [SerializeField] private Gradient _logoColourGradient;

    private bool _isReadyToStart = false;
    private AudioSource _source;


    private void Start()
    {
        OpenHomeMenu();
        _source = GetComponent<AudioSource>();
        if (_source != null)
        {
            _source.playOnAwake = false;
            if (startMusic != null)
            {
                _source.clip = startMusic;
                _source.Play();
                //StartCoroutine(IncreaseVolumeInTime());
            }
        }
        _isReadyToStart = true;
    }
    private void Update()
    {
        if (logo != null && _logoColourGradient != null)
        {
            if (_animateLogoColours)
            {
                AnimateLogoColours();
            }
        }
    }
    private void AnimateLogoColours()
    {
        float t = 0.5f * Mathf.Sin(Time.realtimeSinceStartup) + 0.5f;
        logo.color = _logoColourGradient.Evaluate(t);
    }
    private IEnumerator IncreaseVolumeInTime(float seconds = 11, float finalValue = 0.6f)
    {
        float t = 0;
        while (_source.volume < finalValue)
        {
            yield return null;
            t += Time.deltaTime / seconds;
            _source.volume = Mathf.Lerp(0f, finalValue, t);
        }
    }


    #region Buttons
    public void ButtonStart()
    {
        if (_isReadyToStart)
        {
            SceneManager.LoadScene("Scene1v2.0");
        }
    }
    public void ButtonSettings()
    {
        OpenSettingsMenu();
    }
    public void ButtonQuit()
    {
        // Maybe spawn ask message first
        Application.Quit();
    }
    #endregion

    #region Menus
    public void CloseAllMenus()
    {
        homeMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }
    public void OpenHomeMenu()
    {
        CloseAllMenus();
        homeMenu.SetActive(true);
    }
    public void OpenSettingsMenu()
    {
        CloseAllMenus();
        settingsMenu.SetActive(true);
    }
    #endregion
}
