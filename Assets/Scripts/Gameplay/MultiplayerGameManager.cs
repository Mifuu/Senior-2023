using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
{
    public GameObject playerPrefab;

    public float timeSinceStart = -1;

    [Header("Requirements")]
    public RoomGeneration.RoomGenNetworkManager roomGenNetworkManager;
    public NetworkDebugManager networkDebugManager;

    [Space]
    [SerializeField] public GameObject bossRoomPrefab;

    [Header("Readonly")]
    [ReadOnly] public List<PlayerManager> playerManagers = new List<PlayerManager>();
    [ReadOnly] public List<ulong> playerIds = new List<ulong>();
    [ReadOnly] public Dictionary<ulong, Vector3> playerSpawnPoints = new Dictionary<ulong, Vector3>();
    [ReadOnly] public Vector3[] bossRoomCoords = { new Vector3(-4000, 0, -4000), new Vector3(4000, 0, -4000), new Vector3(-4000, 0, 4000), new Vector3(4000, 0, 4000) };
    NetworkObject[] bossRooms = { null, null, null, null };

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.G))
        {
            StartGame();
        }

        if (timeSinceStart >= 0)
        {
            timeSinceStart += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        if (!IsServer)
        {
            Debug.LogWarning("Not server, no permissions to start game");
            return;
        }

        if (timeSinceStart >= 0)
        {
            Debug.LogWarning("Game already started");
            return;
        }
        timeSinceStart = 0.1f;

        NetworkDebugManager.LogMessage("[MGM] Starting game...");
        NetworkDebugManager.LogMessage("[MGM] Generating level...");
        // request generating level
        roomGenNetworkManager.onGenerateLevel -= OnLevelGenerated;
        roomGenNetworkManager.onGenerateLevel += OnLevelGenerated;
        roomGenNetworkManager.TryGenerate();
    }

    void OnLevelGenerated()
    {
        NetworkDebugManager.LogMessage("[MGM] Spawning players...");

        // spawning player in the correct position
        Debug.Log("Level generated, spawning player...");
        RoomGeneration.RoomGenerator roomGenerator = roomGenNetworkManager.roomGenerator;
        List<Transform> spawnTransform = roomGenerator.GetSpawnTransforms();

        IReadOnlyList<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds;
        playerSpawnPoints = new Dictionary<ulong, Vector3>();

        // Iterate over each client
        for (int i = 0; i < ids.Count; i++)
        {
            ulong id = ids[i];

            Vector3 spawnPosition = Vector3.zero + Vector3.up * 4;
            if (spawnTransform.Count > i)
            {
                spawnPosition = spawnTransform[i].position;
                playerSpawnPoints.Add(id, spawnPosition);
            }
            else
                Debug.LogWarning("No spawn position... Spawning at 0,4,0");

            // SpawnPlayerClientRpc(spawnPosition, id);
            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            NetworkDebugManager.LogMessage("[MGM] Player spawned at " + spawnPosition);
            var n = player.GetComponent<NetworkObject>();
            var p = player.GetComponent<PlayerManager>();

            playerManagers.Add(p);
            playerIds.Add(id);

            n.SpawnWithOwnership(id);

            SetPlayerObjectClientRpc(id);
            TeleportPlayerClientRpc(id, spawnPosition);
            SetPlayerSpawnPointClientRpc(id, spawnPosition);
        }
    }

    void OnDisable()
    {
        roomGenNetworkManager.onGenerateLevel -= OnLevelGenerated;
    }

    [ClientRpc]
    void SetPlayerObjectClientRpc(ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject = PlayerManager.thisClient.GetComponent<NetworkObject>();
        }
    }

    [ClientRpc]
    void TeleportPlayerClientRpc(ulong id, Vector3 pos)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            PlayerManager.thisClient.Teleport(pos);
        }
    }

    [ClientRpc]
    void SetPlayerSpawnPointClientRpc(ulong id, Vector3 pos)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            PlayerManager.thisClient.SetSpawnPoint(pos);
        }
    }
    
    [ClientRpc]
    public void TeleportPlayerToSpawnClientRpc(ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            PlayerManager.thisClient.Teleport(PlayerManager.thisClient.spawnPoint);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportPlayerToSpawnServerRpc(ulong id)
    {
        TeleportPlayerClientRpc(id, playerSpawnPoints[id]);

        if (bossRooms[playerIds.IndexOf(id)] != null)
        {
            bossRooms[playerIds.IndexOf(id)].Despawn(true);
            bossRooms[playerIds.IndexOf(id)] = null;
        }
    }

    [ClientRpc]
    public void RespawnPlayerClientRpc(ulong id)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
        {
            PlayerManager.thisClient.TeleportToSpawnPoint();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TeleportToBossRoomServerRpc(ulong id)
    {
        /* if (NetworkManager.Singleton.LocalClientId == id) */
        /* { */
            Debug.Log("Index: " + playerIds.IndexOf(id) + " Coords: " + bossRoomCoords[playerIds.IndexOf(id)]);
            if (bossRooms[playerIds.IndexOf(id)] != null)
            {
                bossRooms[playerIds.IndexOf(id)].Despawn();
                bossRooms[playerIds.IndexOf(id)] = null;
            }

            GameObject o = Instantiate(bossRoomPrefab, bossRoomCoords[playerIds.IndexOf(id)], Quaternion.identity);
            NetworkObject n = o.GetComponent<NetworkObject>();
            n.Spawn();
            bossRooms[playerIds.IndexOf(id)] = n;
            TeleportToBossRoomClientRpc(id, bossRoomCoords[playerIds.IndexOf(id)]);
        /* } */
    }

    [ClientRpc]
    void TeleportToBossRoomClientRpc(ulong id, Vector3 coord)
    {
        if (NetworkManager.Singleton.LocalClientId == id)
            PlayerManager.thisClient.Teleport(coord);
    }
}
