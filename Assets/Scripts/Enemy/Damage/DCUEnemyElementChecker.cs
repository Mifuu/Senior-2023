using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Elemental Checker", fileName = "ElementalChecker")]
public class DCUEnemyElementChecker : DamageCalculationUnitBase
{
    ElementAppliable appliable;
    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        Debug.LogWarning("Init Element Checker");
        appliable = component.gameObject.GetComponent<ElementAppliable>();
    }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        // Should also adjust the damage
        appliable.TryApplyElement(info.dealer, info.elementalDamageParameter, info.gunType);
        return info;
    }
}
