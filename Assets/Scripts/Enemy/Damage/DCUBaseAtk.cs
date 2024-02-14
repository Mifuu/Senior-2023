using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Base Attack", fileName = "Base Attack")]
    public class DCUBaseAtk : DamageCalculationUnitBase
    {
        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            adder.AddFloat("BaseAtk", component.gameObject.GetComponent<EnemyBase>().BaseAtk);
        }

        public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
        {
            float BaseATK;
            if (getter.GetFloat("BaseAtk", out BaseATK))
            {
                return BaseATK;
            }

            return initialValue;
        }
    }
}

// ENEMY DEALER 
// - Base ATK (amount)
// - Elemental DMG Bonus
//
// ENEMY RECEIVER
// - Base DEF (factor)
// - Elemental RES
// - Weakpoint Damage Factor
// - Gun Type
