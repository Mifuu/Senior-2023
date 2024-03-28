using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public static PlayerManager thisClient;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playersInGame.Value = 0;

        if (IsOwner && IsClient)
        {
            thisClient = this;
        }
    }

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }
    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Debug.Log($"{id} just connected...");
                    playersInGame.Value++;
                }
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Debug.Log($"{id} just disconnected...");
                    playersInGame.Value--;
                }
            };
        }
        else
        {
            Debug.Log("PlayerManager: NetworkManager is null, Starting in offline mode.");
        }
    }
}