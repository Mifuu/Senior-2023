using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour 
{
    public NetworkVariable<float> ElementShard_1 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<float> ElementShard_2 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<float> ElementShard_3 { get; set; } = new NetworkVariable<float>(0.0f);
    public NetworkVariable<float> Key { get; set; } = new NetworkVariable<float>(0.0f);

}
