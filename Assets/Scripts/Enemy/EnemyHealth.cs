using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(NetworkObject))]
public class EnemyHealth : NetworkBehaviour
{
    private Enemy enemy;
    public NetworkVariable<int> HP_stat = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enemy = GetComponent<Enemy>();
        if (IsServer)
        {
            enemy.isReady.OnValueChanged += (_, current) =>
            {
                if (current)
                {
                    HP_stat.Value = enemy.enemyConfig.HP_stat;
                }
            };

            HP_stat.OnValueChanged += (_, current) =>
            {
                if (current <= 0)
                {
                    GetComponent<NetworkObject>().Despawn();
                    Destroy(gameObject);
                }
            };
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Player tag is temp and is used for testing only 
        if (IsServer && collision.gameObject.CompareTag("Player"))
        {
            HP_stat.Value -= 100;
        }
    }
}
