using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private bool PlayMusicOnStart;
    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusicClip;

    private AudioSource m_AudioSource;

    private void Start()
    {
        SetupComponents();   
    }

    private void SetupComponents()
    {
        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            Debug.LogWarning($"No audiosource was found on this gameobject. Adding a new one,");
            m_AudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (m_AudioSource != null)
        {
            float vol;
            if (PlayerPrefs.HasKey("Volume"))
            {
                vol = PlayerPrefs.GetFloat("Volume");
            }
            else
            {
                vol = 0.5f;
            }
            m_AudioSource.playOnAwake = false;
            m_AudioSource.clip = backgroundMusicClip;
            m_AudioSource.loop = true;
            m_AudioSource.volume = vol;
            if (PlayMusicOnStart)
            {
                m_AudioSource.Play();
            }
        }
    }
}
