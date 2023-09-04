using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfx_audio_source;
    [SerializeField] private AudioClip[] sfx_clips;

    public static SoundFxManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void play_hover()
    {
        sfx_audio_source.PlayOneShot(sfx_clips[0], 0.2f);
    }

    public void play_click()
    {
        sfx_audio_source.PlayOneShot(sfx_clips[1], 0.4f);
    }
    
}
