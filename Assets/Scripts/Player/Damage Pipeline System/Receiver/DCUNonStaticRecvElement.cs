using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Receiver/Player Element")]
public class DCUNonStaticRecvElement : DamageCalculationUnitBase
{
    ElementAppliable appliable;
    PlayerStat stat;

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        appliable = component.GetComponent<ElementAppliable>();
        stat = component.GetComponent<PlayerStat>();
    }

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover) { }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        info.amount *= stat.GetElementRES(info.elementalDamageParameter.element);
        return info;
    }
}
