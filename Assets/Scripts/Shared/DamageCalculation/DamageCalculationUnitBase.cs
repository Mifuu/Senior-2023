using UnityEngine;

public abstract class DamageCalculationUnitBase : ScriptableObject
{
    [SerializeField] public string UniqueName;
    [SerializeField] public bool requireInstantiation;
    public abstract void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange);
    public abstract void Dispose(DamageCalculationComponent component, SubscriptionRemover remover);
    public virtual float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue) => initialValue; 
    public virtual DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info) => info; 
}
