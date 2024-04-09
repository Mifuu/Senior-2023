using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Base Damage Receive Factor", fileName = "Base Damage Receive Factor")]
    public class DCUBaseDef : DamageCalculationUnitBase
    {
        [SerializeField] private float defaultEmergencyBaseDef;

        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
            remover.RemoveFloat("BaseDef");
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            adder.AddFloat("BaseDef", component.gameObject.GetComponent<Enemy.EnemyStat>().DamageReceiveFactor);
        }

        public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
        {
            float def;
            if (getter.GetFloat("BaseDef", out def))
            {
                return 1/def;
            }
            return defaultEmergencyBaseDef;
        }
    }
}
