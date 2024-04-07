using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Damage/Player ATK")]
public class DCUStaticPlayerATK : DamageCalculationUnitBase
{
    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
        {
            stat.OnStatsChanged -= component.DealerPipeline.CalculateAndCache;
            buff.OnBuffChanged -= component.DealerPipeline.CalculateAndCache;
        }
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
        {
            stat.OnStatsChanged += component.DealerPipeline.CalculateAndCache;
            buff.OnBuffChanged += component.DealerPipeline.CalculateAndCache;
        }
    }

    public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
            return stat.BaseATK * buff.AtkBuffTotal;
        return initialValue;
    }
}
