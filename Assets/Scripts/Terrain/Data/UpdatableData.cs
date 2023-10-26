using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate)
            NotifyOfUpdatedValues();
    }

    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
            OnValuesUpdated();
    }
}
