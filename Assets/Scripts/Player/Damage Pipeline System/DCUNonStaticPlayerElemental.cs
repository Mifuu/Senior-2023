using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Test Non Static")]
public class DCUNonStaticPlayerElemental : DamageCalculationUnitBase
{
    ElementAttachable attachable;
    ElementalEntity entity;
    PlayerStat stat;

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        attachable = component.GetComponent<ElementAttachable>();
        entity = component.GetComponent<ElementalEntity>();
        stat = component.GetComponent<PlayerStat>();
        Debug.LogWarning("Current Player Damage Calculation Still only use Pistol as gun type, change this later");
    }

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover) { }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        //info.elementalDamageParameter = new ElementalDamageParameter(attachable.element, entity);
        info.amount *= stat.GetElementDMGBonus(info.elementalDamageParameter.element);
        info.gunType = TemporaryGunType.Pistol; // Change this later
        return info;
    }
}
