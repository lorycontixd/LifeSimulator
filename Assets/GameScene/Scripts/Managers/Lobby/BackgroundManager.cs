using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    #region Singleton
    private static BackgroundManager _instance;
    public static BackgroundManager Instance { get { return _instance; } }

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

    public List<Sprite> backgrounds = new List<Sprite>();
    public float backgroundDurationSec;
    public bool CycleBackgrounds;
    public bool Fade = false;
    [SerializeField] private Image backgroundImage;
    private ImageFade imageFade = null;

    private int currentBackgroundIndex = -1;

    private void Start()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        if (imageFade == null)
        {
            imageFade = GetComponent<ImageFade>();
        }
        if (CycleBackgrounds)
        {
            StartCoroutine(StartCycling());
        }
    }
    private IEnumerator StartCycling()
    {
        while (true)
        {
            SetNextImage();
            yield return new WaitForSeconds(backgroundDurationSec);
        }
    }
    private int GetNextIndex()
    {
        return (currentBackgroundIndex + 1) % backgrounds.Count;
    }
    private void SetNextImage()
    {
        int nextIndex = GetNextIndex();
        if (imageFade != null && currentBackgroundIndex != -1 && Fade)
        {
            imageFade.CrossFadeChangebackground(backgroundImage, backgrounds[nextIndex]);
        }
        else
        {
            backgroundImage.sprite = backgrounds[nextIndex];
        }
        currentBackgroundIndex = nextIndex;
        
    }

}
