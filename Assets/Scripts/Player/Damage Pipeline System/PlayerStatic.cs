using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Test Static")]
public class PlayerStatic : DamageCalculationUnitBase
{
    PlayerStat stat;

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        stat = component.GetComponent<PlayerStat>();
    }

    public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
    {
        return stat.BaseATK;
    }
}
