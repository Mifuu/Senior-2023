using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Elemental Checker", fileName = "ElementalChecker")]
public class DCUEnemyElementChecker : DamageCalculationUnitBase
{
    ElementAppliable appliable;
    Enemy.EnemyStat stat;

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        appliable = component.gameObject.GetComponent<ElementAppliable>();
        stat = component.gameObject.GetComponent<Enemy.EnemyStat>();
    }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        info.amount *= stat.GetElementalRESValue(info.elementalDamageParameter.element);
        appliable.TryApplyElement(info.dealer, info.elementalDamageParameter, info.gunType);
        return info;
    }
}
