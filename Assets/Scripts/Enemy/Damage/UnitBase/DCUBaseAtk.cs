using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Base Attack", fileName = "Base Attack")]
    public class DCUBaseAtk : DamageCalculationUnitBase
    {
        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
            remover.RemoveFloat("BaseDMG");
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            adder.AddFloat("BaseDMG", component.gameObject.GetComponent<EnemyBase>().stat.BaseDamage);
        }

        public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
        {
            float BaseATK;
            if (getter.GetFloat("BaseDMG", out BaseATK))
            {
                return BaseATK;
            }

            return initialValue;
        }
    }
}
