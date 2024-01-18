using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LODRendererAssigner))]
public class LODRendererAssignerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LODRendererAssigner lodRendererAssigner = (LODRendererAssigner)target;

        if (GUILayout.Button("Clear Renderer"))
            lodRendererAssigner.ClearRenderer();
        if (GUILayout.Button("Assign Children Renderers"))
            lodRendererAssigner.AssignChildrenRenderers();
    }
}
