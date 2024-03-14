using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public SFXList sfxList;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (sfxList == null)
        {
            Debug.LogError("[ERROR]SFXManager: SFXList is null, please add one to the SFXManager!");
        }
    }

    /// <summary>
    /// Plays a sound from the list of sounds to the target
    /// </summary>
    public void PlaySFX(string name, GameObject target)
    {
        if (Instance.sfxList == null)
        {
            Debug.LogError("[Error]SFXManager: SFXList is null, please add one to the SFXManager!");
            return;
        }

        if (!Array.Exists(sfxList.sounds, sound => sound.name == name))
        {
            Debug.LogError($"[Error]SFXManager: The sound corresponding to {name} doesn't exist!");
            return;
        }

        // find sound (In the future, we can use a dictionary to store the sounds for faster access)
        SFXSound s = Array.Find(sfxList.sounds, sound => sound.name == name);

        // ensuring source
        AudioSource source = target.GetComponent<AudioSource>();
        if (source == null)
        {
            source = target.AddComponent<AudioSource>();
            if (s.audioMixerGroup != null)
                source.outputAudioMixerGroup = s.audioMixerGroup;

            source.maxDistance = 30f;
            source.minDistance = 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1f;
        }

        if (source.enabled) source.PlayOneShot(s.clip, s.volume);
    }

    /// <summary>
    /// Plays a random sound from the list of sounds to the target
    /// </summary>
    public void PlaySFX(string[] names, GameObject target)
    {
        int i = UnityEngine.Random.Range(0, names.Length);
        PlaySFX(names[i], target);
    }

    /// <summary>
    /// Plays a sound from the list of sounds to the target
    /// </summary>
    public static void TryPlaySFX(string name, GameObject target)
    {
        if (Instance == null)
        {
            Debug.LogError("[Error]SFXManager: SFXManager IS NULL, please add one into the scene from prefab folder!");
            return;
        }

        Instance.PlaySFX(name, target);
    }

    /// <summary>
    /// Plays a random sound from the list of sounds to the target
    /// </summary>
    public static void TryPlaySFX(string[] names, GameObject target)
    {
        int i = UnityEngine.Random.Range(0, names.Length);
        TryPlaySFX(names[i], target);
    }
}
