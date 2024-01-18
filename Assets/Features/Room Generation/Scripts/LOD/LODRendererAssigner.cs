using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LODGroup))]
public class LODRendererAssigner : MonoBehaviour
{
    public LODGroup lodGroup = null;

    void OnValidate()
    {
        if (lodGroup == null && TryGetComponent(out LODGroup _lodGroup))
            lodGroup = _lodGroup;
    }

    public void ClearRenderer()
    {
        LOD[] lods = lodGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            lods[i].renderers = new Renderer[0];
        }
        lodGroup.SetLODs(lods);
    }

    public void AssignChildrenRenderers()
    {
        LOD[] lods = lodGroup.GetLODs();
        lods[0].renderers = GetMeshRenderersInChildren(transform).ToArray();
        lodGroup.SetLODs(lods);
    }

    private List<MeshRenderer> GetMeshRenderersInChildren(Transform transform)
    {
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
        if (transform.TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderers.Add(meshRenderer);
        foreach (Transform child in transform)
        {
            meshRenderers.AddRange(GetMeshRenderersInChildren(child));
        }
        return meshRenderers;
    }
}
