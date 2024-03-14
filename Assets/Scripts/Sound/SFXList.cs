using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SFXList", menuName = "Sound/SFXList", order = 1)]
public class SFXList : ScriptableObject
{
    public SFXSound[] sounds;
}

[System.Serializable]
public class SFXSound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 4f)]
    public float volume = 1;
    public AudioMixerGroup audioMixerGroup;
}