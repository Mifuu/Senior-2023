using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RoomGeneration.Minimap
{
    [CustomEditor(typeof(MinimapDisplay))]
    public class MinimapDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MinimapDisplay minimapDisplay = (MinimapDisplay)target;

            if (GUILayout.Button("Generate"))
            {
                minimapDisplay.Generate();
            }
        }
    }
}

