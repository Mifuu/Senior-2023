using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : NetworkBehaviour
{
    private Enemy enemy;
    private NetworkVariable<int> HP_stat = new NetworkVariable<int>(10);

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
        }

        HP_stat.OnValueChanged += (_, current) => 
        {
            if (IsServer && current <= 0)
            {
                Destroy(gameObject);
            }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Bullet"))
        {
            HP_stat.Value -= 10;
        }
    }
}
