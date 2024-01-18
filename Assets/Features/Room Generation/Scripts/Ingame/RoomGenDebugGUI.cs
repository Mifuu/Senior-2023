using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenDebugGUI : MonoBehaviour
{
    void OnGUI()
    {
        if (!DebugGUIManager.active)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 400, Screen.height / 3 + 10, 400, Screen.height / 3));
        GUILayout.Label("[RoomGenNetworkDebugGUI]");
        if (RoomGenNetworkManager.Instance == null)
        {
            GUILayout.Label("RoomGenNetworkManager.Instance == null");
        }
        else
        {
            if (!RoomGenNetworkManager.Instance.IsServer)
            {
                GUILayout.Label("Not a server, no permissions to generate Room");
            }
            else
            {
                if (GUILayout.Button("Generate Terrain"))
                    RoomGenNetworkManager.Instance.TryGenerate();
            }
        }
        GUILayout.EndArea();
    }
}
