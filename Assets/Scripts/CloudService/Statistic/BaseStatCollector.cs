using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseStatCollector : ScriptableObject
{
    public static BaseStatCollector Singleton;

    [Serializable]
    public struct NetworkStat
    {
        [SerializeField] public string name;
        [SerializeField] public int defaultValue;
    }

    [SerializeField] public List<NetworkStat> listOfStats;
    private Dictionary<string, object> valueDict = new Dictionary<string, object>();

    public void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else 
            Destroy(this);
    }

    public Dictionary<string, object> PushStatToStatService() => valueDict;

    public List<NetworkStat> PullStatFromStatService(Dictionary<string, object> statDicts)
    {
        List<NetworkStat> nonExistingStat = new List<NetworkStat>();
        Dictionary<string, object> stat = new Dictionary<string, object>();
        foreach (NetworkStat focusedStat in listOfStats)
        {
            if (statDicts.TryGetValue(focusedStat.name, out object value))
                stat.Add(focusedStat.name, value);
            else
                nonExistingStat.Add(focusedStat);
        }

        return nonExistingStat;
    }

    public bool Set<T>(string key, T value)
    {
        if (valueDict.TryGetValue(key, out object returnedValue)) return false;
        valueDict[key] = value;
        return true;
    }

    public bool Set<T>(string key, Func<T, T> action) 
    {
        if (valueDict.TryGetValue(key, out object value)) return false;
        var result = action((T) value);
        valueDict[key] = result;
        return true;
    }
}
