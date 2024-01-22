using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class RoomGenDebugGUI : MonoBehaviour
{
    public RoomGenNetworkManager roomGenNetworkManager;
    public RoomGenSpawnEnemy roomGenSpawnEnemy;

    void OnGUI()
    {
        if (!DebugGUIManager.active)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 400, Screen.height / 3 + 10, 400, Screen.height / 3));
        GUILayout.Label("[RoomGenNetworkDebugGUI]");
        if (roomGenNetworkManager == null)
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
                if (GUILayout.Button("Generate Level (CTRL + M)"))
                {
                    if (roomGenNetworkManager == null)
                    {
                        Debug.Log("RoomGenNetworkManager Not Found");
                        return;
                    }
                    roomGenNetworkManager.TryGenerate();
                }
                if (GUILayout.Button("Spawn Enemy (CTRL + N)"))
                {
                    if (roomGenSpawnEnemy == null)
                    {
                        Debug.Log("RoomGenSpawnEnemy Not Found");
                        return;
                    }
                    roomGenSpawnEnemy.ImidiateSpawnEnemy();
                }
            }
        }
        GUILayout.EndArea();
    }
}
