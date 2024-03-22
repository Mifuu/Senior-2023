using UnityEngine;
using Unity.Netcode;

public class ElementalReactionCollectible : NetworkBehaviour
{
    public enum ItemType
    {
        Plus,
        Minus,
        Multiply,
        Divide,
    }

    [SerializeField] private ItemType itemType;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsPlayerObject)
        {
            switch (itemType)
            {
                case ItemType.Plus:
                    Debug.Log("Picking Up Plus collectible");
                    break;
                case ItemType.Minus:
                    Debug.Log("Picking Up Minus collectible");
                    break;
                case ItemType.Multiply:
                    Debug.Log("Picking Up Multiply collectible");
                    break;
                case ItemType.Divide:
                    Debug.Log("Picking Up Divide collectible");
                    break;
            }
            
            NetworkObject.Despawn(true);
        }
    }
}
