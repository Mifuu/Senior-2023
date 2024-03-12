using UnityEngine;
using Unity.Netcode;

public class PlayerHitboxDamageable : NetworkBehaviour, IDamageCalculatable
{
    private IDamageable damageable;
    public ulong playerId;
    [SerializeField] private float simpleDamageFactor = 1.0f;
    private DamageCalculationComponent damageCalculationComponent;

    private void Awake()
    {
        damageable = GetComponentInParent<IDamageable>(true);
        damageCalculationComponent = GetComponentInParent<DamageCalculationComponent>();
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
        return damageCalculationComponent.GetFinalDealthDamageAmount(damageInfo) * simpleDamageFactor;
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (!IsServer) return;
        var trueDamageAmount = CalculateDamage(damageInfo);
        damageable.Damage(trueDamageAmount, damageInfo.dealer);
        // Debug.LogWarning("Player received: " + trueDamageAmount);
    }

    public bool HasMatchingPlayerId(ulong receivedPlayerId)
    {
        if (playerId == receivedPlayerId) return true;
        else { return false; }
    }

    public float getCurrentHealth() => damageable.currentHealth.Value;
}
