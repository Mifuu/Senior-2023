using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(NetworkObject))]
public class EnemyHealth : NetworkBehaviour
{
    private Enemy _enemy;
    public NetworkVariable<int> hpStat = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _enemy = GetComponent<Enemy>();
        if (!IsServer) return;
        _enemy.isReady.OnValueChanged += (_, current) =>
        {
            if (current)
            {
                hpStat.Value = _enemy.enemyConfig.hpStat;
            }
        };

        hpStat.OnValueChanged += (_, current) =>
        {
            if (current > 0) return;
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer && collision.gameObject.CompareTag("Attack"))
        {
            hpStat.Value -= 100;
        }
    }
}
