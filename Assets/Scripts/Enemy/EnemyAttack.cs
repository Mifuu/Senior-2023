using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAttack : NetworkBehaviour 
{
    [SerializeField] private GameObject bulletPrefab; 
    private readonly NetworkVariable<bool> _isShooting = new NetworkVariable<bool>();
    private Enemy _enemy;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _enemy = GetComponent<Enemy>();
        if (_enemy == null)
        {
            enabled = false;
            Debug.LogError("Enemy is null");
            return;
        }

        if (!IsServer) return;
        _isShooting.Value = true;
        _enemy.isReady.OnValueChanged += (_, current) =>
        {
            if (!current) return;
            enabled = true;
        };

        _isShooting.OnValueChanged += ((_, current) =>
        {
            if (!current) return;
            StartCoroutine(StartShooting());
        });
    }

    private void Update()
    {
        transform.LookAt(_enemy.followingPlayer.transform);
    }

    private IEnumerator StartShooting()
    {
        if (IsServer)
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * 50.0f, ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(5.0f);
    }
}