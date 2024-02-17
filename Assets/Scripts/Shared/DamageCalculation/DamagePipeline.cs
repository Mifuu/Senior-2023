using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class DamagePipeline
{
    [SerializeField] public float DefaultValue { get; set; }
    [SerializeField] public float CachedValue { get; private set; }
    [SerializeField] public List<DamageCalculationUnitBase> StaticUnits = new List<DamageCalculationUnitBase>();
    [SerializeField] public List<DamageCalculationUnitBase> NonStaticUnits = new List<DamageCalculationUnitBase>();

    public event Action<float, float> EmitOnCacheCalculateEvent;
    [HideInInspector] public DamageCalculationComponent managerModule;

    public SubscriptionGetter getter;
    public SubscriptionAdder adder;
    public SubscriptionRemover remover;

    private GameObject gameObject;
    private bool isDealer;

    public void InitializePipeline(DamageCalculationComponent managerModule, GameObject gameObject, bool isDealer)
    {
        this.managerModule = managerModule;
        this.gameObject = gameObject;
        this.isDealer = isDealer;

        adder = new SubscriptionAdder("", isDealer, managerModule.subscriptionContainer);
        remover = new SubscriptionRemover("", isDealer, managerModule.subscriptionContainer);
        getter = new SubscriptionGetter("", isDealer, managerModule.subscriptionContainer);

        for (int i = 0; i < StaticUnits.Count; i++)
        {
            adder.UnitName = StaticUnits[i].UniqueName;
            if (StaticUnits[i].requireInstantiation)
            {
                var instantiatedModule = GameObject.Instantiate(StaticUnits[i]);
                instantiatedModule.Initialize(managerModule, adder, true);
                StaticUnits[i] = instantiatedModule;
                continue;
            }

            StaticUnits[i].Initialize(managerModule, adder, true);
        }

        for (int i = 0; i < NonStaticUnits.Count; i++)
        {
            adder.UnitName = NonStaticUnits[i].UniqueName;
            if (NonStaticUnits[i].requireInstantiation)
            {
                var instantiatedModule = GameObject.Instantiate(NonStaticUnits[i]);
                instantiatedModule.Initialize(managerModule, adder, false);
                NonStaticUnits[i] = instantiatedModule;
                continue;
            }

            NonStaticUnits[i].Initialize(managerModule, adder, false);
        }

        CalculateAndCache();
    }

    public void Dispose()
    {
        foreach (var modules in StaticUnits)
        {
            remover.UnitName = modules.UniqueName;
            modules.Dispose(managerModule, remover);
        }

        foreach (var modules in NonStaticUnits)
        {
            remover.UnitName = modules.UniqueName;
            modules.Dispose(managerModule, remover);
        }
    }

    public void CalculateAndCache()
    {
        var oldCache = CachedValue;
        CachedValue = StaticUnits.Aggregate(DefaultValue, (aggregatedDamage, next) =>
        {
            // if (!next.IsEnabled) return aggregatedDamage;
            getter.UnitName = next.UniqueName;
            return next.CalculateCache(managerModule, getter, aggregatedDamage);
        });

        EmitOnCacheCalculateEvent?.Invoke(oldCache, CachedValue);
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
        adder.UnitName = unit.UniqueName;
        unit.Initialize(managerModule, adder, false);
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

    [Obsolete("Temporary Dont use this")]
    public bool ChangeUnitEnableState(DamageCalculationUnitBase unit, bool isStatic, bool newState)
    {
        var foundUnit = FindUnit(unit, isStatic);
        if (foundUnit != null)
        {
            // foundUnit.IsEnabled = newState;
            return true;
        }
        return false;
    }

    public DamageInfo GetValueInfo(DamageInfo info = new DamageInfo())
    {
        info.amount = CachedValue;
        return NonStaticUnits.Aggregate(info, (aggregatedDamage, next) =>
        {
            // if (!next.IsEnabled) return aggregatedDamage;
            getter.UnitName = next.UniqueName;
            return next.CalculateActual(managerModule, getter, aggregatedDamage);
        });
    }

    public float GetValue(DamageInfo info = new DamageInfo()) => GetValueInfo(info).amount;
}
