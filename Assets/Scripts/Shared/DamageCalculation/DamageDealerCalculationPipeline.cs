using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DamageDealerCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedDamage { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    [SerializeField] public List<IDamageCalculationUnitBase> StaticModules = new List<IDamageCalculationUnitBase>();
    [SerializeField] public List<IDamageCalculationUnitBase> NonStaticModules = new List<IDamageCalculationUnitBase>();

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
            instantiatedModule.Initialize(this, false, gameObject);
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
