using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Element", fileName = "EnemyElementalDamage")]
    public class DCUEnemyElementalDamage : DamageCalculationUnitBase
    {
        ElementalEntity elementalEntity;
        ElementAttachable elementAttached;
        Enemy.EnemyStat enemyStat;

        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover) { }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            elementalEntity = component.gameObject.GetComponent<ElementalEntity>();
            elementAttached = component.gameObject.GetComponent<ElementAttachable>();
            enemyStat = component.gameObject.GetComponent<EnemyStat>();
        }

        public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
        {
            info.elementalDamageParameter = new ElementalDamageParameter(elementAttached.element, elementalEntity);
            info.amount *= enemyStat.GetElementalDMGValue(elementAttached.element);
            return info;
        }
    }
}
