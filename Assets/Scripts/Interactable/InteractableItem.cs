using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class InteractableItem : NetworkBehaviour
{
    public string promptMessage;

    public void BaseInteract(ulong PlayerId)
    {
        Interact(PlayerId);
    }

    protected virtual void Interact(ulong PlayerId)
    {
        
    }

    /*
    [ServerRpc]
    private void InteractServerRpc(ulong PlayerId)
    {
        Interact(PlayerId);
    }
    */
}
