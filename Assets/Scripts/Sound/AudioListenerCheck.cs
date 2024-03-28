using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AudioListenerCheck : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!GetParentNetworkObject().IsLocalPlayer)
        {
            AudioListener audioListener = GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
    }

    NetworkObject GetParentNetworkObject()
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            NetworkObject networkObject = parent.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                return networkObject;
            }
            parent = parent.parent;
        }
        return null;
    }
}
