using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainNetworkDebugGUI : MonoBehaviour
{
    void OnGUI()
    {
        if (!DebugGUIManager.active)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 400, Screen.height / 3 + 10, 400, Screen.height / 3));
        GUILayout.Label("[TerrainNetworkDebugGUI]");
        if (TerrainNetworkManager.Instance == null)
        {
            GUILayout.Label("TerrainNetworkManager.Instance == null");
        }
        else
        {
            if (!TerrainNetworkManager.Instance.IsServer)
            {
                GUILayout.Label("Not a server, no permissions to generate terrain");
            }
            else
            {
                if (GUILayout.Button("Generate Terrain"))
                    TerrainNetworkManager.Instance.TryGenerate();
                // if (GUILayout.Button("Spawn Enemy"))
                    // EnemySpawnManager.Instance.SetIsSpawn(prev => !prev);
            }
        }
        GUILayout.EndArea();
    }
}
