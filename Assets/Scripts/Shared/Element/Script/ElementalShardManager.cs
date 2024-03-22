using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ElementalShardManager : NetworkBehaviour
{
    public static ElementalShardManager Singleton { get; private set; }
    [SerializeField] private List<ElementalShardMap> shardMap;
    private Dictionary<ElementalType, GameObject> shardDict = new Dictionary<ElementalType, GameObject>();

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        foreach (var map in shardMap)
            shardDict.Add(map.type, map.shardPrefab);
    }

    public GameObject GetShardOfElement(ElementalType element) => shardDict[element];
}

[Serializable]
public struct ElementalShardMap
{
    [SerializeField] public ElementalType type;
    [SerializeField] public GameObject shardPrefab;
}
