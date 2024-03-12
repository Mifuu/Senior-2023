using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    public AudioSource audioSource;

    public Music backgroundMusic;
    public Music bossMusic;

    void Start()
    {
        Play(backgroundMusic);
    }

    public void Play(Music music)
    {
        audioSource.clip = music.clip;
        audioSource.volume = music.volume;
        audioSource.outputAudioMixerGroup = music.audioMixerGroup;
        audioSource.Play();
    }

    public static void PlayBackgroundMusic()
    {
        if (Instance == null)
        {
            Debug.Log("[Error no singleton instance for MusicManager");
            return;
        }
        Instance.Play(Instance.backgroundMusic);
    }

    public static void PlayBossMusic()
    {
        if (Instance == null)
        {
            Debug.Log("[Error no singleton instance for MusicManager");
            return;
        }
        Instance.Play(Instance.bossMusic);
    }

    void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}

[System.Serializable]
public class Music
{
    public AudioClip clip;

    [Range(0f, 4f)]
    public float volume = 1;
    public AudioMixerGroup audioMixerGroup;
}