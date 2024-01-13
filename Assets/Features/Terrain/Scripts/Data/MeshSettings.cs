using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/MeshSettings")]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    public static readonly int[] supportedFlatshadedChunkSizes = { 48, 72, 96 };

    public float meshScale = 1;
    public bool useFlatShading;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    public int flatshadedChunkSizeIndex;

    // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 2 - 1;
        }
    }

    public float meshWorldSize
    {
        get
        {   // subtract 1 from edges count instead of vertex, subtract 2 to negate 2 extra verts
            return ((numVertsPerLine - 1) - 2) * meshScale;
        }
    }
}
