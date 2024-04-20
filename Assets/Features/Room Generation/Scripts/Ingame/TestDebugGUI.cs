using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Enemy;

public class TestDebugGUI : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerManager _PlayerManager
    {
        get
        {
            if (playerManager == null)
                playerManager = PlayerManager.thisClient;
            return playerManager;
        }
    }

    private PlayerInventory playerInventory;
    private PlayerInventory _PlayerInventory
    {
        get
        {
            if (playerInventory == null)
                playerInventory = _PlayerManager.GetComponent<PlayerInventory>();
            return playerInventory;
        }
    }

    private PlayerLevel playerLevel;
    private PlayerLevel _PlayerLevel
    {
        get
        {
            if (playerLevel == null)
                playerLevel = _PlayerManager.GetComponent<PlayerLevel>();
            return playerLevel;
        }
    }

    void OnGUI()
    {
        if (!DebugGUIManager.active)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 400, Screen.height / 3 + 10, 400, Screen.height / 2));
        GUILayout.Label("[TestDebugGUI]");

        if (!NetworkManager.Singleton.IsClient)
        {
            GUILayout.Label("Not a Client");
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.Label("Items");
            // water, fire, earth, wind
            if (GUILayout.Button("Water Shard + 10"))
                _PlayerInventory.AddWaterShardServerRpc(10);
            if (GUILayout.Button("Fire Shard + 10"))
                _PlayerInventory.AddFireShardServerRpc(10);
            if (GUILayout.Button("Earth Shard + 10"))
                _PlayerInventory.AddEarthShardServerRpc(10);
            if (GUILayout.Button("Wind Shard + 10"))
                _PlayerInventory.AddWindShardServerRpc(10);
            if (GUILayout.Button("Key + 1"))
                _PlayerInventory.AddKeyServerRpc(1);

            GUILayout.Space(10);
            GUILayout.Label("Level");
            if (GUILayout.Button("Exp + 100"))
                _PlayerLevel.AddExp(100);
            if (GUILayout.Button("Exp + 500"))
                _PlayerLevel.AddExp(500);
            if (GUILayout.Button("Exp + 1000"))
                _PlayerLevel.AddExp(1000);

            GUILayout.Space(10);
            GUILayout.Label("Damage");
            if (GUILayout.Button("30m Radius - 500 HP"))
            {
                Collider[] colliders = Physics.OverlapSphere(_PlayerManager.transform.position, 30f, LayerMask.GetMask("Enemy"));
                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out IDamageCalculatable damageable))
                    {
                        DamageInfo damageInfo = new(_PlayerManager.gameObject, 500);
                        damageable.Damage(damageInfo);
                        Debug.Log("Damage dealt to " + collider.gameObject.name + ": " + damageInfo.amount);
                    }
                }
            }

            /* not working properly
            GUILayout.Space(10);
            GUILayout.Label("Scene");
            if (GUILayout.Button("Reload Scene"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if (GUILayout.Button("Main Menu"))
                SceneManager.LoadScene("MainMenu");
            */
        }
        GUILayout.EndArea();
    }
}