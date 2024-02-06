using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DamageDealerCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedDamage { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    [SerializeField] public List<DamageCalculationUnitBase> StaticModules = new List<DamageCalculationUnitBase>();
    [SerializeField] public List<DamageCalculationUnitBase> NonStaticModules = new List<DamageCalculationUnitBase>();

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
                instantiatedModule.Initialize(this, false, gameObject);
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
        CachedDamage = StaticModules.Aggregate(DefaultValue, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.PreCalculate(aggregatedDamage);
        });
    }

    public DamageInfo GetFinalDealthDamageInfo(DamageInfo info = new DamageInfo())
    {
        info.amount = CachedDamage;
        return NonStaticModules.Aggregate(info, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float GetFinalDealthDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalDealthDamageInfo(info).amount;
}
