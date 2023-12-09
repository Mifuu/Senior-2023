using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomDataPlotter))]
public class RoomDataPlotterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoomDataPlotter roomDataPlotter = (RoomDataPlotter)target;

        GUILayout.Space(25);

        if (GUILayout.Button("Add Collider"))
        {
            roomDataPlotter.AddCollider();
        }
        if (GUILayout.Button("Update Colliders"))
        {
            roomDataPlotter.UpdateColliderData();
        }
    }
}
