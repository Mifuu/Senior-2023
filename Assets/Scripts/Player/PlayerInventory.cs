using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public NetworkVariable<float> ElementShard_1 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<float> ElementShard_2 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<float> ElementShard_3 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<int> Key { get; set; } = new NetworkVariable<int>(0);

    //public event Action OnInventoryChanged;

    [ServerRpc]
    public void IncreaseKeyServerRpc(ServerRpcParams rpcParams = default)
    {
        Key.Value += 1;
        //OnInventoryChanged?.Invoke();
    }

    [ServerRpc]
    public void IncreaseElementShard_1ServerRpc(ServerRpcParams rpcParams = default)
    {
        ElementShard_1.Value += 1;
        //OnInventoryChanged?.Invoke();
    }

    [ServerRpc]
    public void IncreaseElementShard_2ServerRpc(ServerRpcParams rpcParams = default)
    {
        ElementShard_2.Value += 1;
        //OnInventoryChanged?.Invoke();
    }

    [ServerRpc]
    public void IncreaseElementShard_3ServerRpc(ServerRpcParams rpcParams = default)
    {
        ElementShard_3.Value += 1;
        //OnInventoryChanged?.Invoke();
    }
}
