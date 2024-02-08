using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayUI
{
    [CustomEditor(typeof(GameplayUIManager))]
    public class GameplayUIManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameplayUIManager manager = (GameplayUIManager)target;

            YEditorUtility.BestGirlBanner();

            DrawDefaultInspector();
        }
    }
}