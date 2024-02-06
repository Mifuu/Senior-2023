using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DamageReceiverCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedFactor { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    [SerializeField] public List<DamageCalculationUnitBase> StaticModules { get; set; } = new List<DamageCalculationUnitBase>();
    [SerializeField] public List<DamageCalculationUnitBase> NonStaticModules { get; set; } = new List<DamageCalculationUnitBase>();

    public void Start()
    {
        for (int i = 0; i < StaticModules.Count; i++)
        {
            if (StaticModules[i].requireInstantiation)
            {
                var instantiatedModule = Instantiate(StaticModules[i]); // Caution: Instantiating a ScriptableObject, might impact performance
                instantiatedModule.Initialize(this, true, gameObject);
                StaticModules[i] = instantiatedModule;
                continue;
            }

            StaticModules[i].Initialize(this, true, gameObject);
        }

        for (int i = 0; i < NonStaticModules.Count; i++)
        {
            if (NonStaticModules[i].requireInstantiation)
            {
                var instantiatedModule = Instantiate(NonStaticModules[i]); // Caution: Instantiating a ScriptableObject, might impact performance
                instantiatedModule.Initialize(this, true, gameObject);
                NonStaticModules[i] = instantiatedModule;
                continue;
            }

            NonStaticModules[i].Initialize(this, false, gameObject);
        }

        CalculateAndCache();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var modules in StaticModules)
        {
            modules.Dispose();
        }

        foreach (var modules in NonStaticModules)
        {
            modules.Dispose();
        }
    }

    public DamageCalculationUnitBase AddUnit(DamageCalculationUnitBase unit, bool isStatic)
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
