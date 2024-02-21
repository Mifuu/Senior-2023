using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Key_Interactable : InteractableItem
{
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (!IsServer) return;
        GameObject playerObject = other.gameObject;
        if (playerObject != null) // Check if the collieded object is a player
        {
            NetworkBehaviour networkBehaviour = other.GetComponent<NetworkBehaviour>();
            if (networkBehaviour != null) // Check if the player has a networkBehavior component
            {
                ulong playerId = networkBehaviour.NetworkObjectId;
                GiveKeyServerRpc(playerId);
            }
        }
        */
        // Check if the colliding object is the player
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsPlayerObject)
        {
            // Call a server RPC on the player to increase the key count
            if (other.TryGetComponent<PlayerInventory>(out var playerInventory))
            {
                playerInventory.IncreaseKeyServerRpc();
                NetworkObject.Despawn(true);
            }
        }
    }

    public void Spawn()
    {
        // Check if we are the server before spawning the gun
        if (NetworkManager.Singleton.IsServer)
        {
            // Spawn the key prefab
            NetworkObject key = Instantiate(NetworkObject, transform.position, transform.rotation).GetComponent<NetworkObject>();
            key.Spawn();
        }
    }

    /*
    [ServerRpc]
    public void GiveKeyServerRpc(ulong PlayerId)
    {
        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(PlayerId).gameObject;
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>;
    }
    */
}
