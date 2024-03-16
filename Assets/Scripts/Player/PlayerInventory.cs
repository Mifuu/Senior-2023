using System;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    #region collectible items
    public NetworkVariable<int> Key { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> WaterShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> FireShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> EarthShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> WindShard { get; set; } = new NetworkVariable<int>(0);
    #endregion

    [ServerRpc]
    public void AddKeyServerRpc(int i)
    {
        Key.Value += i;
    }

    [ServerRpc]
    public void AddWaterShardServerRpc(int i)
    {
        WaterShard.Value += i;
    }

    [ServerRpc]
    public void AddFireShardServerRpc(int i)
    {
        FireShard.Value += i;
    }

    [ServerRpc]
    public void AddEarthShardServerRpc(int i)
    {
        EarthShard.Value += i;
    }

    [ServerRpc]
    public void AddWindShardServerRpc(int i)
    {
        WindShard.Value += i;
    }
}