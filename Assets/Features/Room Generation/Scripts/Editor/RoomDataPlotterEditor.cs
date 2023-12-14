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
            roomDataPlotter.AddCollider();
        GUILayout.Space(10);
        if (GUILayout.Button("Update Colliders"))
            roomDataPlotter.UpdateColliderData();
        if (GUILayout.Button("Get Room Box Data"))
            roomDataPlotter.GetRoomBoxData();

        GUILayout.Space(25);
        GUILayout.Label(roomDataPlotter.latestRoomBoxData);
    }
}
