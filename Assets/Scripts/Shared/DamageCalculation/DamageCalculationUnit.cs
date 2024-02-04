using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class DamageCalculationUnit<T> : IDamageCalculationUnitBase
{
    private IDamageCalculationPipelineBase PipelineBase;
    private Dictionary<string, ObserverPattern.IObservable<T>> ListOfSubject { get; set; } = new Dictionary<string, ObserverPattern.IObservable<T>>();
    private bool _isEnabled = true;
    protected GameObject gameObject;

    public override bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            PipelineBase.CalculateAndCache();
        }
    }

    private void TriggerRecalculate_Internal(T prev, T current) => PipelineBase.CalculateAndCache();

    public override void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange, GameObject owner)
    {
        if (pipelineBase != null) Debug.LogWarning("Initialize() is being called on already initiated DamageCalculationUnit");

        PipelineBase = pipelineBase;
        gameObject = owner;

        if (updateOnChange)
        {
            foreach (KeyValuePair<string, ObserverPattern.IObservable<T>> valuePair in ListOfSubject)
            {
                valuePair.Value.OnValueChanged += TriggerRecalculate_Internal;
            }
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
        subject.OnValueChanged += TriggerRecalculate_Internal;
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
