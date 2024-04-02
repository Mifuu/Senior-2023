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
                if (GUILayout.Button("Enemy Test Group Spawn"))
                    Enemy.EnemySpawnManager.Singleton.TestGroupSpawn(3);
                // if (GUILayout.Button("Enemy Spawn Selected At location"))
                //     Enemy.EnemySpawnManager.Singleton.SpawnSelectedEnemyAtSelectedLocation();
                if (GUILayout.Button("Enemy Spawn Selected"))
                    Enemy.EnemySpawnManager.Singleton.SpawnSelectedEnemy();
                if (GUILayout.Button("Spawn Boss Room"))
                    Enemy.EnemySpawnManager.Singleton.SpawnBossRoom();
            }
        }
        GUILayout.EndArea();
    }
}
