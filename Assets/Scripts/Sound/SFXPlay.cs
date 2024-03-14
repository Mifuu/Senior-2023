using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlay : MonoBehaviour
{
    public string sfxName;

    public void Activate()
    {
        PlayName(sfxName);
    }

    public void Activate(GameObject source)
    {
        PlayName(sfxName, gameObject);
    }

    public void PlayName(string name, GameObject source = null)
    {
        string[] sfxNames = name.Split(',');
        foreach (var sfx in sfxNames)
        {
            sfx.Trim();
        }
        if (source == null) source = gameObject;
        SFXManager.TryPlaySFX(sfxNames, source);
    }
}
