using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
{
    public GameObject playerPrefab;

    [HideInInspector]
    public List<GameObject> playerList = new List<GameObject>();

    public float timeSinceStart = -1;

    [Header("Requirements")]
    public RoomGeneration.RoomGenNetworkManager roomGenNetworkManager;
    public NetworkDebugManager networkDebugManager;

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
        if (timeSinceStart >= 0)
        {
            Debug.LogWarning("Game already started");
            return;
        }
        timeSinceStart = 0.1f;

        if (!IsServer)
        {
            Debug.LogWarning("Not server, no permissions to start game");
        }

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

        // Iterate over each client
        for (int i = 0; i < ids.Count; i++)
        {
            ulong id = ids[i];

            Vector3 spawnPosition = Vector3.zero;
            if (spawnTransform.Count > i)
                spawnPosition = spawnTransform[i].position;
            else
                Debug.LogWarning("No spawn position... Spawning at 0,0,0");


            GameObject player = Instantiate(playerPrefab, spawnTransform[0].position, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(id);
            playerList.Add(player);
        }
    }

    void OnDisable()
    {
        roomGenNetworkManager.onGenerateLevel -= OnLevelGenerated;
    }
}
