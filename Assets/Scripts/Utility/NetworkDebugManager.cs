using UnityEngine;
using Unity.Netcode;

public class NetworkDebugManager : NetworkBehaviour
{
    public static NetworkDebugManager instance;

    void Awake()
    {
        instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    private void LogMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        Debug.Log("[NetworkDebugManager:Server]: " + message);

        // Forward the message to all clients
        LogMessageClientRpc(message);
    }

    [ClientRpc]
    private void LogMessageClientRpc(string message)
    {
        // Log the message on all clients
        Debug.Log("[NetworkDebugManager:Client]: " + message);
    }

    public static void LogMessage(string message)
    {
        if (instance == null)
        {
            // create instance
            GameObject o = new GameObject("NetworkDebugManager");
            instance = o.AddComponent<NetworkDebugManager>();
        }
        if (NetworkManager.Singleton.IsServer)
        {
            // Log the message on the server
            Debug.Log("[NetworkDebugManager:Server]: " + message);
            instance.LogMessageClientRpc(message);
        }
        else
        {
            // Log the message on the client
            instance.LogMessageServerRpc(message);
        }
    }
}
