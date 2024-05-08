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

    public void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else 
            Destroy(this);
    }

#if DEDICATED_SERVER

    private Dictionary<string, Dictionary<string, object>> playerStats;

    public List<NetworkStat> PullStatFromStatService(string playerId, Dictionary<string, object> statDicts)
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

        playerStats.Add(playerId, stat);
        return nonExistingStat;
    }

    public Dictionary<string, object> PushStatToStatService(string playerId) => playerStats[playerId];

    public bool Set<T>(ulong playerNetworkId, string key, T value)
    {
        var playerId = CloudService.StatService.Singleton.networkIdMapper[playerNetworkId];
        if (!playerStats.TryGetValue(playerId, out Dictionary<string, object> returnedPlayerStat) 
                || !returnedPlayerStat.TryGetValue(key, out object returnedValue)) return false;
        playerStats[playerId][key] = value;
        return true;
    }

    public bool Set<T>(ulong playerNetworkId, string key, Func<T, T> action) 
    {
        var playerId = CloudService.StatService.Singleton.networkIdMapper[playerNetworkId];
        if (!playerStats.TryGetValue(playerId, out Dictionary<string, object> returnedPlayerStat)
                    || !returnedPlayerStat.TryGetValue(key, out object value)) return false;
        var result = action((T) value);
        playerStats[playerId][key] = result;
        return true;
    }
#endif
}
