using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isMoving = new NetworkVariable<bool>();
    [SerializeField] public NetworkVariable<bool> isReady = new NetworkVariable<bool>();
    private readonly NetworkVariable<int> _enemyConfigId = new NetworkVariable<int>(-1); // crucial must be set to non zero
    public EnemyScriptableObject enemyConfig;
    GameObject _followingPlayer;

    void Update()
    {
        if (!isMoving.Value || !isReady.Value) return;
        Vector3 direction = (_followingPlayer.transform.position - transform.position).normalized;
        direction.y = 0f;
        transform.Translate(direction * (enemyConfig.movementSpdStat * Time.deltaTime));
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _enemyConfigId.OnValueChanged += (_, _) =>
        {
            Initialize();
        };
    }

    public void InitializeConfigId(int id)
    {
        if (IsServer)
        {
            _enemyConfigId.Value = id;
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        foreach (GameObject player in players)
        {
            Vector3 distance = transform.position - player.transform.position;
            if (closestPlayer == null || closestPlayer.transform.position.sqrMagnitude > distance.sqrMagnitude)
            {
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    private void Initialize()
    {
        _followingPlayer = FindClosestPlayer();
        enemyConfig = EnemySpawnManager.Instance.GetEnemyConfigById(_enemyConfigId.Value);
        GetComponent<Renderer>().material = enemyConfig.mat;

        if (IsServer)
        {
            isMoving.Value = _followingPlayer != null;
        }

        if (enemyConfig != null && _followingPlayer != null)
        {
            isReady.Value = true;
        }
    }
}
