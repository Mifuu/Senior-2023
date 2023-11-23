using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isMoving = new NetworkVariable<bool>();
    [SerializeField] public NetworkVariable<bool> isReady = new NetworkVariable<bool>();
    private readonly NetworkVariable<int> _enemyConfigId = new NetworkVariable<int>(-1); // crucial must be set to non zero
    public EnemyScriptableObject enemyConfig;
    public GameObject followingPlayer;

    void Update()
    {
        if (!isMoving.Value || !isReady.Value) return;
        if ((transform.position - followingPlayer.transform.position).sqrMagnitude > 50.0f || enemyConfig.isMelee)
            transform.Translate((transform.position - followingPlayer.transform.position).normalized * (enemyConfig.movementSpdStat * Time.deltaTime));
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
        followingPlayer = FindClosestPlayer();
        enemyConfig = EnemySpawnManager.Instance.GetEnemyConfigById(_enemyConfigId.Value);
        GetComponent<Renderer>().material = enemyConfig.mat;

        if (IsServer)
        {
            isMoving.Value = followingPlayer != null;
        }

        if (enemyConfig != null && followingPlayer != null)
        {
            isReady.Value = true;
        }
    }
}
