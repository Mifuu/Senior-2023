using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class DamageReceiverCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedFactor { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    public List<IDamageCalculationUnitBase> StaticModules { get; set; } = new List<IDamageCalculationUnitBase>();
    public List<IDamageCalculationUnitBase> NonStaticModules { get; set; } = new List<IDamageCalculationUnitBase>();

    public void Start()
    {
        foreach (var module in StaticModules)
        {
            module.Initialize(this, true);
        }

        foreach (var module in NonStaticModules)
        {
            module.Initialize(this, false);
        }
    }

    public IDamageCalculationUnitBase AddUnit(IDamageCalculationUnitBase unit, bool isStatic)
    {
        if (isStatic) 
        {
            unit.Initialize(this, true);
            StaticModules.Add(unit);
            return unit;
        }

        unit.Initialize(this, false);
        NonStaticModules.Add(unit);
        return unit;
    }

    public void CalculateAndCache()
    {
        CachedFactor = StaticModules.Aggregate(DefaultValue, (aggregatedFactor, next) =>
        {
            if (!next.isEnabled) return aggregatedFactor;
            return next.PreCalculate(aggregatedFactor);
        });
    }

    public DamageInfo GetFinalReceivedDamageInfo(DamageInfo info)
    {
        info.amount = info.amount * CachedFactor;
        return NonStaticModules.Aggregate(info, (aggregatedDamage, next) =>
        {
            if (!next.isEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float GetFinalReceivedDamageAmount(DamageInfo info) => GetFinalReceivedDamageInfo(info).amount;
}
