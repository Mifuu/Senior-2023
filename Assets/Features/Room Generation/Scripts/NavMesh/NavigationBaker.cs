using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavigationBaker : MonoBehaviour
{
    public NavMeshSurface[] surfaces;

    [ContextMenu("Bake NavMesh")]
    public void BakeNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}
