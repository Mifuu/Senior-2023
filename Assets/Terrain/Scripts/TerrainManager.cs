using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : Singleton<TerrainManager>
{
    [Header("Settings")]
    public bool generateOnStart = false;
    private int seed = 0;

    [Header("Requirements")]
    public EndlessTerrain endlessTerrain;
    public MapGenerator mapGenerator;

    public void Start()
    {
        if (generateOnStart)
            Generate();
    }

    public void SetViewer(Transform viewer)
    {
        endlessTerrain.viewer = viewer;
    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
        mapGenerator.gameplaySeed = seed;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        endlessTerrain.Clear();
        endlessTerrain.Init();
    }
}
