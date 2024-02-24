using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Player ATK")]
public class DCUStaticPlayerATK : DamageCalculationUnitBase
{
    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
        PlayerStat stat;
        if (!component.TryGetComponent<PlayerStat>(out stat)) return;
        stat.OnStatsChanged -= component.DealerPipeline.CalculateAndCache;
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        PlayerStat stat;
        if (!component.TryGetComponent<PlayerStat>(out stat)) return;
        stat.OnStatsChanged += component.DealerPipeline.CalculateAndCache;
    }

    public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
    {
        PlayerStat stat;
        if (component.TryGetComponent<PlayerStat>(out stat))
        {
            return stat.BaseATK;
        }
        return initialValue;
    }
}
