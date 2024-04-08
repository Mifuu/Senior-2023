using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Receiver/Player Defence")]
public class DCUStaticPlayerDef : DamageCalculationUnitBase
{
    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
        {
            stat.OnStatsChanged += component.ReceiverPipeline.CalculateAndCache;
            buff.OnBuffChanged += component.ReceiverPipeline.CalculateAndCache;
        }
        else
            Debug.LogError("BuffManager and PlayerStat not found");
    }

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
        {
            stat.OnStatsChanged -= component.ReceiverPipeline.CalculateAndCache;
            buff.OnBuffChanged -= component.ReceiverPipeline.CalculateAndCache;
        }
    }

    public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
    {
        if (component.TryGetComponent<PlayerStat>(out PlayerStat stat) && component.TryGetComponent<BuffManager>(out BuffManager buff))
            return stat.BaseDEF * buff.DefBuffTotal;
        return initialValue;
    }
}
