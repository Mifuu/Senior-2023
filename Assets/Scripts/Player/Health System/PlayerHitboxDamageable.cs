using UnityEngine;
using Unity.Netcode;

public class PlayerHitboxDamageable : NetworkBehaviour, IDamageCalculatable
{
    private IDamageable damageable;
    public ulong playerId;
    [SerializeField] private float simpleDamageFactor = 1.0f;

    private void Awake()
    {
        damageable = GetComponentInParent<IDamageable>(true);
        if (damageable == null)
        {
            Debug.LogError("Player Script: IDamageable Not Found");
        }
    }

    public void InitializePlayerId(ulong receivedPlayerId)
    {
        playerId = receivedPlayerId;
    }

    protected virtual float CalculateDamage(DamageInfo damageInfo)
    {
        return simpleDamageFactor * damageInfo.amount;
    }

    public void Damage(DamageInfo damageInfo)
    {
        var trueDamageAmount = CalculateDamage(damageInfo);
        damageable.Damage(trueDamageAmount, damageInfo.dealer);
    }

    public bool HasMatchingPlayerId(ulong receivedPlayerId)
    {
        if (playerId == receivedPlayerId) return true;
        else { return false; }
    }

    public float getCurrentHealth() => damageable.currentHealth.Value;
}
