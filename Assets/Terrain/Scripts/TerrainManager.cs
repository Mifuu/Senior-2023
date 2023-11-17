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

    public void Update()
    {
        // should be change to listener method
        if (Camera.main.transform != endlessTerrain.viewer)
            endlessTerrain.viewer = Camera.main.transform;
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
