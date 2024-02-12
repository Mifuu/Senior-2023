using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using System;

public class DamageDealerCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedDamage { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    [SerializeField] public List<DamageCalculationUnitBase> StaticUnits = new List<DamageCalculationUnitBase>();
    [SerializeField] public List<DamageCalculationUnitBase> NonStaticUnits = new List<DamageCalculationUnitBase>();
    public event Action<float, float> OnCacheCalculate;

    public void Start()
    {
        for (int i = 0; i < StaticUnits.Count; i++)
        {
            if (StaticUnits[i].requireInstantiation)
            {
                var instantiatedModule = Instantiate(StaticUnits[i]);
                instantiatedModule.Initialize(this, true, gameObject);
                StaticUnits[i] = instantiatedModule;
                continue;
            }

            StaticUnits[i].Initialize(this, true, gameObject);
        }

        for (int i = 0; i < NonStaticUnits.Count; i++)
        {
            if (NonStaticUnits[i].requireInstantiation)
            {
                var instantiatedModule = Instantiate(NonStaticUnits[i]);
                instantiatedModule.Initialize(this, false, gameObject);
                NonStaticUnits[i] = instantiatedModule;
                continue;
            }

            NonStaticUnits[i].Initialize(this, false, gameObject);
        }

        CalculateAndCache();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var modules in StaticUnits)
        {
            modules.Dispose();
        }

        foreach (var modules in NonStaticUnits)
        {
            modules.Dispose();
        }
    }

    public DamageCalculationUnitBase FindUnit(DamageCalculationUnitBase unit, bool isStatic)
    {
        if (isStatic)
        {
            return StaticUnits.Find(unitInList => unit == unitInList);
        }
        return NonStaticUnits.Find(unitInList => unit == unitInList);
    }

    public DamageCalculationUnitBase AddUnit(DamageCalculationUnitBase unit, bool isStatic)
    {
        unit.Initialize(this, isStatic, gameObject);
        if (isStatic)
        {
            StaticUnits.Add(unit);
        }
        else
        {
            NonStaticUnits.Add(unit);
        }
        return unit;
    }

    public bool RemoveUnit(DamageCalculationUnitBase unit, bool isStatic)  
    {
        if (isStatic)
        {
            return StaticUnits.Remove(unit);
        }
        return NonStaticUnits.Remove(unit);
    }
    

    public bool ChangeUnitEnableState(DamageCalculationUnitBase unit, bool isStatic, bool newState)
    {
        var foundUnit = FindUnit(unit, isStatic);
        if (foundUnit != null)
        {
            foundUnit.IsEnabled = newState;    
            return true;
        }
        return false;
    }

    public void CalculateAndCache()
    {
        var oldCache = CachedDamage;
        CachedDamage = StaticUnits.Aggregate(DefaultValue, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.PreCalculate(aggregatedDamage);
        });

        OnCacheCalculate?.Invoke(oldCache, CachedDamage);
    }

    public DamageInfo GetFinalDealthDamageInfo(DamageInfo info = new DamageInfo())
    {
        info.amount = CachedDamage;
        return NonStaticUnits.Aggregate(info, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float GetFinalDealthDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalDealthDamageInfo(info).amount;
}
