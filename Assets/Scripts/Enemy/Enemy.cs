using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isMoving = new NetworkVariable<bool>();
    [SerializeField] public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false);
    private NetworkVariable<int> enemyConfigId = new NetworkVariable<int>(-1); // crucial must be set to non zero
    public EnemyScriptableObject enemyConfig;
    GameObject followingPlayer;

    void Update()
    {
        if (isMoving.Value && isReady.Value && followingPlayer != null)
        {
            Vector3 direction = (followingPlayer.transform.position - transform.position).normalized;
            direction.y = 0f;
            transform.Translate(direction
               * enemyConfig.Movement_SPD_stat * Time.deltaTime);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enemyConfigId.OnValueChanged += (_, current) =>
        {
            Initialize();
        };
    }

    public void InitializeConfigId(int id)
    {
        if (IsServer)
        {
            enemyConfigId.Value = id;
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
        followingPlayer = FindClosestPlayer();
        enemyConfig = EnemySpawnManager.Instance.GetEnemyConfigById(enemyConfigId.Value);
        GetComponent<Renderer>().material = enemyConfig.mat;

        if (IsServer)
        {
            isMoving.Value = followingPlayer == null ? false : true;
        }

        if (enemyConfig != null && followingPlayer != null)
        {
            isReady.Value = true;
        }
    }
}
