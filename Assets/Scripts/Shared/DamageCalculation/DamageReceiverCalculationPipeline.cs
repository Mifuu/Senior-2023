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
        for (int i = 0; i < StaticModules.Count; i++)
        {
            var instantiatedModule = Instantiate(StaticModules[i]); // Caution: Instantiating a ScriptableObject, might impact performance
            instantiatedModule.Initialize(this, true, gameObject);
            StaticModules[i] = instantiatedModule;
        }

        for (int i = 0; i < NonStaticModules.Count; i++)
        {
            var instantiatedModule = Instantiate(StaticModules[i]); // Caution: Instantiating a ScriptableObject, might impact performance
            instantiatedModule.Initialize(this, true, gameObject);
            NonStaticModules[i] = instantiatedModule;
        }

        CalculateAndCache();
    }

    public IDamageCalculationUnitBase AddUnit(IDamageCalculationUnitBase unit, bool isStatic)
    {
        if (isStatic) 
        {
            unit.Initialize(this, true, gameObject);
            StaticModules.Add(unit);
            return unit;
        }

        unit.Initialize(this, false, gameObject);
        NonStaticModules.Add(unit);
        return unit;
    }

    public void CalculateAndCache()
    {
        CachedFactor = StaticModules.Aggregate(DefaultValue, (aggregatedFactor, next) =>
        {
            if (!next.IsEnabled) return aggregatedFactor;
            return next.PreCalculate(aggregatedFactor);
        });
    }

    public DamageInfo GetFinalReceivedDamageInfo(DamageInfo info)
    {
        info.amount = info.amount * CachedFactor;
        return NonStaticModules.Aggregate(info, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float GetFinalReceivedDamageAmount(DamageInfo info) => GetFinalReceivedDamageInfo(info).amount;
}
