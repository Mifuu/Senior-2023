using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseV1
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // check if scale is out of range
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // iterates for each coordinates
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                // sampling perlin noise
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}
