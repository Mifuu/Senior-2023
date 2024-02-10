using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DamageReceiverCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedFactor { get; protected set; }
    public float DefaultValue { get; set; } = 1;
    [SerializeField] public List<DamageCalculationUnitBase> StaticUnits { get; set; } = new List<DamageCalculationUnitBase>();
    [SerializeField] public List<DamageCalculationUnitBase> NonStaticUnits { get; set; } = new List<DamageCalculationUnitBase>();

    public void Start()
    {
        for (int i = 0; i < StaticUnits.Count; i++)
        {
            if (StaticUnits[i].requireInstantiation)
            {
                var instantiatedModule = Instantiate(StaticUnits[i]); // Caution: Instantiating a ScriptableObject, might impact performance
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
                var instantiatedModule = Instantiate(NonStaticUnits[i]); // Caution: Instantiating a ScriptableObject, might impact performance
                instantiatedModule.Initialize(this, true, gameObject);
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
        if (isStatic)
        {
            unit.Initialize(this, true, gameObject);
            StaticUnits.Add(unit);
            return unit;
        }

        unit.Initialize(this, false, gameObject);
        NonStaticUnits.Add(unit);
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
        CachedFactor = StaticUnits.Aggregate(DefaultValue, (aggregatedFactor, next) =>
        {
            if (!next.IsEnabled) return aggregatedFactor;
            return next.PreCalculate(aggregatedFactor);
        });
    }

    public DamageInfo GetFinalReceivedDamageInfo(DamageInfo info)
    {
        info.amount = info.amount * CachedFactor;
        return NonStaticUnits.Aggregate(info, (aggregatedDamage, next) =>
        {
            if (!next.IsEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float GetFinalReceivedDamageAmount(DamageInfo info) => GetFinalReceivedDamageInfo(info).amount;
}
