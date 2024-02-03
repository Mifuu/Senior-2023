using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class DamageCalculationUnit<T> : IDamageCalculationUnitBase
{
    private IDamageCalculationPipelineBase PipelineBase;
    private Dictionary<string, ObserverPattern.IObservable<T>> ListOfSubject { get; set; }

    public abstract float Calculate(float initialValue);

    public bool isEnabled
    {
        get => isEnabled;
        set 
        {
            isEnabled = value;
            PipelineBase.CalculateAndCache();
        }
    }

    private void TriggerRecalculate_Internal(T prev, T current) => PipelineBase.CalculateAndCache();

    public void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange)
    {
        if (pipelineBase != null) Debug.LogWarning("Initialize() is being called on already initiated DamageCalculationUnit");
        PipelineBase = pipelineBase;

        if (!updateOnChange) return;
        foreach (KeyValuePair<string, ObserverPattern.IObservable<T>> valuePair in ListOfSubject)
        {
            valuePair.Value.OnValueChanged += TriggerRecalculate_Internal;
        }
    }

    public void Dispose()
    {
        foreach (KeyValuePair<string, ObserverPattern.IObservable<T>> valuePair in ListOfSubject)
        {
            valuePair.Value.OnValueChanged -= TriggerRecalculate_Internal;
        }
    }

    public void AddParameter(string name, ObserverPattern.Subject<T> subject)
    {
        ListOfSubject.Add(name, subject);
    }

    public ObserverPattern.IObservable<T> TryGetParameterSubject(string key)
    {
        ObserverPattern.IObservable<T> subject;
        var isGetResultValid = ListOfSubject.TryGetValue(key, out subject);
        if (isGetResultValid)
        {
            return subject;
        }

        return null;
        throw new Exception("Subject of key: " + key + " can not be found");
    }

    public T TryGetParameter(string key)
    {
        try
        {
            var tryResult = TryGetParameterSubject(key);
            if (tryResult != null)
            {
                return tryResult.Value;
            }

            return default(T);
        }
        catch
        {
            throw;
        }
    }
}
