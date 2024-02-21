using UnityEngine;

[CreateAssetMenu(fileName = "TrivialValueSetting", menuName = "Enemy/Enemy Damage Unit/Value Setter")]
public class DCUTrivialValueStter : DamageCalculationUnitBase
{
    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
    {
    }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
    }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        info.gunType = TemporaryGunType.None;
        info.dealer = component.gameObject;
        return info;
    }
}
