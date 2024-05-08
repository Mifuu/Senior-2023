using Unity.Netcode;
using UnityEngine;

public class CollectibleItem : InteractableItem
{
    public enum ItemType
    {
        Key,
        WaterShard,
        FireShard,
        EarthShard,
        WindShard
    }

    public ItemType itemType;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        Debug.Log(transform.name + " collided with player");
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player collided with {name}");

            // Call the appropriate player inventory method based on the item type
            if (other.TryGetComponent<PlayerInventory>(out var playerInventory))
            {
                switch (itemType)
                {
                    case ItemType.Key:
                        playerInventory.AddKeyServerRpc(1);
                        break;
                    case ItemType.WaterShard:
                        playerInventory.AddWaterShardServerRpc(1);
                        break;
                    case ItemType.FireShard:
                        playerInventory.AddFireShardServerRpc(1);
                        break;
                    case ItemType.EarthShard:
                        playerInventory.AddEarthShardServerRpc(1);
                        break;
                    case ItemType.WindShard:
                        playerInventory.AddWindShardServerRpc(1);
                        break;
                    default:
                        Debug.LogError($"Unhandled item type: {itemType}");
                        break;
                }
                NetworkObject.Despawn(true);
            }
            else
            {
                Debug.LogError("Player inventory component not found on the colliding object.");
            }
        }
        else
        {
            Debug.Log("Collieded object is not player");
        }
    }
}