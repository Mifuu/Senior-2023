using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomDataPlotter)), CanEditMultipleObjects]
public class RoomDataPlotterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("This script is used to plot the room data in the scene view.\n" +
                   "Require gizmos to be turned on in the scene view.");

        DrawDefaultInspector();
        RoomDataPlotter roomDataPlotter = (RoomDataPlotter)target;

        if (!roomDataPlotter.IsPlotting)
            return;

        GUILayout.Space(25);

        if (GUILayout.Button("Add Room Box"))
            roomDataPlotter.AddRoomBox();
        if (GUILayout.Button("Update Room Box"))
            roomDataPlotter.UpdateRoomBox();

        GUILayout.Space(10);
        if (GUILayout.Button("Add Room Door"))
            roomDataPlotter.AddRoomDoor();
        if (GUILayout.Button("Update Room Door"))
            roomDataPlotter.UpdateRoomDoor();

        GUILayout.Space(10);
        if (GUILayout.Button("Get Room Box Data"))
            roomDataPlotter.GetRoomBoxData();
        if (GUILayout.Button("Get Room Door Data"))
            roomDataPlotter.GetRoomDoorData();

        GUILayout.Space(10);
        if (GUILayout.Button("Update and Get All Data"))
        {
            if (targets.Length <= 1)
            {
                roomDataPlotter.UpdateRoomBox();
                roomDataPlotter.UpdateRoomDoor();
                roomDataPlotter.GetRoomBoxData();
                roomDataPlotter.GetRoomDoorData();
            }
            else
            {
                foreach (var t in targets)
                {
                    RoomDataPlotter rdp = (RoomDataPlotter)t;
                    rdp.UpdateRoomBox();
                    rdp.UpdateRoomDoor();
                    rdp.GetRoomBoxData();
                    rdp.GetRoomDoorData();
                }
            }
        }

        GUILayout.Space(25);
        GUILayout.Label(roomDataPlotter.latestRoomBoxData);
        GUILayout.Label(roomDataPlotter.latestRoomDoorData);
    }
}
