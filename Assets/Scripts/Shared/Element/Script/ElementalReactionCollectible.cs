using UnityEngine;
using Unity.Netcode;
using System;

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
        Debug.Log("Picking Up Orb Collectible");
        if (!IsServer) return;
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.TryGetComponent<PlayerStat>(out var stat))
        {
            switch (itemType)
            {
                case ItemType.Plus:
                    if (networkObject.TryGetComponent<PlayerHealth>(out var health))
                        health.currentHealth.Value *= 1.02f;
                    Debug.Log("Picking Up Plus collectible");
                    break;
                case ItemType.Minus:
                    stat.BaseATK *= 1.02f;
                    Debug.Log("Picking Up Minus collectible");
                    break;
                case ItemType.Multiply:
                    Array values = Enum.GetValues(typeof(ElementalType));
                    System.Random random = new System.Random();
                    ElementalType randomBar = (ElementalType)values.GetValue(random.Next(values.Length));
                    var bonus = stat.GetElementDMGBonus(randomBar);
                    stat.SetElementDMGBonud(randomBar, bonus * 1.02f);
                    Debug.Log("Picking Up Multiply collectible");
                    break;
                case ItemType.Divide:
                    stat.BaseDEF *= 1.02f;
                    Debug.Log("Picking Up Divide collectible");
                    break;
            }

            NetworkObject.Despawn(true);
        }
    }
}
