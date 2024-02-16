using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Element", fileName = "EnemyElementalDamage")]
    public class DCUEnemyElementalDamage : DamageCalculationUnitBase
    {
        ElementalEntity elementalEntity;
        ElementAttachable elementAttached;

        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            elementalEntity = component.gameObject.GetComponent<ElementalEntity>();
            elementAttached = component.gameObject.GetComponent<ElementAttachable>();
        }

        public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
        {
            // Calculating Elemental Damage Bonus is omitted for now
            info.elementalDamageParameter = new ElementalDamageParameter(elementAttached.element, elementalEntity);
            return info;
        }
    }
}
