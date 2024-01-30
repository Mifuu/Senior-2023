using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RoomGeneration
{
    // editor for room generator
    [CustomEditor(typeof(RoomGenerator))]
    public class RoomGeneratorEditor : Editor
    {
        int bulkAmount = 1;

        public override void OnInspectorGUI()
        {
            GUILayout.Label("This script is used to generate the room in the scene view.\n" +
                       "Require gizmos to be turned on in the scene view.");

            DrawDefaultInspector();
            RoomGenerator roomGenerator = (RoomGenerator)target;

            GUILayout.Space(25);
            GUILayout.Label("Debug Step Generation");

            if (GUILayout.Button("Clear"))
                roomGenerator.Clear();
            if (GUILayout.Button("Step Add Room"))
                roomGenerator.StepAddRoom();

            GUILayout.Space(25);
            GUILayout.Label("Debug Bulk Generation");
            bulkAmount = EditorGUILayout.IntField("Amount: ", bulkAmount);
            if (bulkAmount < 1) bulkAmount = 1;
            if (bulkAmount > 1000) bulkAmount = 1000;
            if (GUILayout.Button("Bulk Add Room"))
            {
                roomGenerator.StepAddRoom(bulkAmount);
            }
        }
    }
}