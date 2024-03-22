using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Key_Interactable : InteractableItem
{
    [Obsolete("This script is outdated")]
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsPlayerObject)
        {
            Debug.Log("Key collided with " +  networkObject.name);
            // Call a server RPC on the player to increase the key count
            if (other.TryGetComponent<PlayerInventory>(out var playerInventory))
            {
                playerInventory.AddKeyServerRpc(1);
                NetworkObject.Despawn(true);
            }
        }
    }
}
