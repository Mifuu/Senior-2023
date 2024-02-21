using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InteractableItem : NetworkBehaviour
{
    [SerializeField] public GameObject interactableItemPrefab;
    public string promptMessage;

    public void BaseInteract(ulong PlayerId)
    {
        Interact(PlayerId);
    }

    protected virtual void Interact(ulong PlayerId)
    {
        
    }

    // for spawn testing
    public void Spawn()
    {
        // Check if we are the server before spawning the item
        if (NetworkManager.Singleton.IsServer)
        {
            // Spawn the item prefab
            NetworkObject item = Instantiate(interactableItemPrefab, transform.position, transform.rotation).GetComponent<NetworkObject>();
            item.Spawn();
        }
    }

    /*
    [ServerRpc]
    private void InteractServerRpc(ulong PlayerId)
    {
        Interact(PlayerId);
    }
    */
}
