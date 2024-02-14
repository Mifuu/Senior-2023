using System.Collections.Generic;
using UnityEngine;

public class DamageSubscriptionContainer<T, K, U, V>
{
    public DamageSubscriptionContainer(DamageCalculationComponent component)
    {
        this.component = component;
    }

    private DamageCalculationComponent component;

    private Dictionary<string, ObserverPattern.IObservable<T>> ListT = new Dictionary<string, ObserverPattern.IObservable<T>>();
    private Dictionary<string, ObserverPattern.IObservable<K>> ListK = new Dictionary<string, ObserverPattern.IObservable<K>>();
    private Dictionary<string, ObserverPattern.IObservable<U>> ListU = new Dictionary<string, ObserverPattern.IObservable<U>>();
    private Dictionary<string, ObserverPattern.IObservable<V>> ListV = new Dictionary<string, ObserverPattern.IObservable<V>>();

    public string GenerateKey(string unitName, string variableName) => unitName + "_" + variableName;

    public bool GetTrackedSubject(string UnitName, string name, out ObserverPattern.IObservable<T> value)
        => ListT.TryGetValue(GenerateKey(UnitName, name), out value);
    public bool GetTrackedSubject(string UnitName, string name, out ObserverPattern.IObservable<K> value)
        => ListK.TryGetValue(GenerateKey(UnitName, name), out value);
    public bool GetTrackedSubject(string UnitName, string name, out ObserverPattern.IObservable<U> value)
        => ListU.TryGetValue(GenerateKey(UnitName, name), out value);
    public bool GetTrackedSubject(string UnitName, string name, out ObserverPattern.IObservable<V> value)
        => ListV.TryGetValue(GenerateKey(UnitName, name), out value);

    public void RecalculateDealer(T _, T __) => component.DealerPipeline.CalculateAndCache();
    public void RecalculateDealer(K _, K __) => component.DealerPipeline.CalculateAndCache();
    public void RecalculateDealer(U _, U __) => component.DealerPipeline.CalculateAndCache();
    public void RecalculateDealer(V _, V __) => component.DealerPipeline.CalculateAndCache();
    public void RecalculateReceiver(T _, T __) => component.ReceiverPipeline.CalculateAndCache();
    public void RecalculateReceiver(K _, K __) => component.ReceiverPipeline.CalculateAndCache();
    public void RecalculateReceiver(U _, U __) => component.ReceiverPipeline.CalculateAndCache();
    public void RecalculateReceiver(V _, V __) => component.ReceiverPipeline.CalculateAndCache();

    public bool AddParameterToTrackList(string UnitName, bool isDealerPipeline, string name, ObserverPattern.IObservable<T> subject)
    {
        var key = GenerateKey(UnitName, name);

        if (isDealerPipeline)
        {
            subject.OnValueChanged += RecalculateDealer;
        }
        else
        {
            subject.OnValueChanged += RecalculateReceiver;
        }

        return ListT.TryAdd(key, subject);
    }

    public bool AddParameterToTrackList(string UnitName, bool isDealerPipeline, string name, ObserverPattern.IObservable<K> subject)
    {
        var key = GenerateKey(UnitName, name);

        if (isDealerPipeline)
        {
            subject.OnValueChanged += RecalculateDealer;
        }
        else
        {
            subject.OnValueChanged += RecalculateReceiver;
        }

        return ListK.TryAdd(name, subject);
    }

    public bool AddParameterToTrackList(string UnitName, bool isDealerPipeline, string name, ObserverPattern.IObservable<U> subject)
    {
        var key = GenerateKey(UnitName, name);

        if (isDealerPipeline)
        {
            subject.OnValueChanged += RecalculateDealer;
        }
        else
        {
            subject.OnValueChanged += RecalculateReceiver;
        }

        return ListU.TryAdd(name, subject);
    }

    public bool AddParameterToTrackList(string UnitName, bool isDealerPipeline, string name, ObserverPattern.IObservable<V> subject)
    {
        var key = GenerateKey(UnitName, name);

        if (isDealerPipeline)
        {
            subject.OnValueChanged += RecalculateDealer;
        }
        else
        {
            subject.OnValueChanged += RecalculateReceiver;
        }

        return ListV.TryAdd(name, subject);
    }

    public bool GetTrackedValue(string UnitName, string name, out T value)
    {
        ObserverPattern.IObservable<T> temp;
        var success = GetTrackedSubject(UnitName, name, out temp);
        value = temp.Value;
        return success;
    }

    public bool GetTrackedValue(string UnitName, string name, out K value)
    {
        ObserverPattern.IObservable<K> temp;
        var success = GetTrackedSubject(UnitName, name, out temp);
        value = temp.Value;
        return success;
    }

    public bool GetTrackedValue(string UnitName, string name, out U value)
    {
        ObserverPattern.IObservable<U> temp;
        var success = GetTrackedSubject(UnitName, name, out temp);
        value = temp.Value;
        return success;
    }

    public bool GetTrackedValue(string UnitName, string name, out V value)
    {
        ObserverPattern.IObservable<V> temp;
        var success = GetTrackedSubject(UnitName, name, out temp);
        value = temp.Value;
        return success;
    }

    public bool RemoveTrackedValue(string UnitName, bool isDealerPipeline, string name, out T value)
    {
        value = default(T);
        ObserverPattern.IObservable<T> temp;
        if (ListT.TryGetValue(GenerateKey(UnitName, name), out temp))
        {
            if (isDealerPipeline)
            {
                temp.OnValueChanged -= RecalculateDealer;
            }
            else
            {
                temp.OnValueChanged -= RecalculateReceiver;
            }

        }
        return ListT.Remove(GenerateKey(UnitName, name));
    }

    public bool RemoveTrackedValue(string UnitName, bool isDealerPipeline, string name, out K value)
    {
        value = default(K);
        ObserverPattern.IObservable<K> temp;
        if (ListK.TryGetValue(GenerateKey(UnitName, name), out temp))
        {
            if (isDealerPipeline)
            {
                temp.OnValueChanged -= RecalculateDealer;
            }
            else
            {
                temp.OnValueChanged -= RecalculateReceiver;
            }

        }
        return ListK.Remove(GenerateKey(UnitName, name));
    }

    public bool RemoveTrackedValue(string UnitName, bool isDealerPipeline, string name, out U value)
    {
        value = default(U);
        ObserverPattern.IObservable<U> temp;
        if (ListU.TryGetValue(GenerateKey(UnitName, name), out temp))
        {
            if (isDealerPipeline)
            {
                temp.OnValueChanged -= RecalculateDealer;
            }
            else
            {
                temp.OnValueChanged -= RecalculateReceiver;
            }

        }
        return ListU.Remove(GenerateKey(UnitName, name));
    }

    public bool RemoveTrackedValue(string UnitName, bool isDealerPipeline, string name, out V value)
    {
        value = default(V);
        ObserverPattern.IObservable<V> temp;
        if (ListV.TryGetValue(GenerateKey(UnitName, name), out temp))
        {
            if (isDealerPipeline)
            {
                temp.OnValueChanged -= RecalculateDealer;
            }
            else
            {
                temp.OnValueChanged -= RecalculateReceiver;
            }

        }
        return ListU.Remove(GenerateKey(UnitName, name));
    }
}
