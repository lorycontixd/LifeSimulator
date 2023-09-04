using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public enum SoundMode
    {
        NONE,
        RANDOM,
        ON
    }
    [SerializeField] private AudioClip sound;
    [SerializeField] private SoundMode soundMode;
    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        animator.SetBool("flying", true);

        if (sound != null)
        {
            if (soundMode == SoundMode.ON)
            {
                audioSource.clip = sound;
                audioSource.Play();
            }else if (soundMode == SoundMode.RANDOM)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    audioSource.clip = sound;
                    audioSource.Play();
                }
            }
            else
            {

            }
        }
    }
}
