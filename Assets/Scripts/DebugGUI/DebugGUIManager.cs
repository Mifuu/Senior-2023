using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUIManager : MonoBehaviour
{
    public static bool active = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))
            active = !active;
    }

    void OnGUI()
    {
        if (!active)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label("[DebugGUIManager]");
            GUILayout.Label("Press ';' to toggle debug GUIs");
            GUILayout.EndArea();
        }
    }
}
