using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyHitboxDamageable : NetworkBehaviour, IDamageCalculatable
    {
        private EnemyBase enemy;
        private DamageCalculationComponent damageComponent;

        [SerializeField] private float simpleDamageFactor = 1.0f;

        public void Start()
        {
            enemy = GetComponentInParent<EnemyBase>(true);
            damageComponent = GetComponentInParent<DamageCalculationComponent>();
            if (enemy == null || damageComponent == null)
            {
                Debug.LogError("------------------------------------------------");
                Debug.LogError("Enemy Hitbox Setup Error");
                Debug.LogError(gameObject + " Parent: " + transform.parent.gameObject);
                Debug.LogError(gameObject + " Enemy Base Class: " + enemy);
                Debug.LogError(gameObject + " Damage Component Class: " + damageComponent);
                Debug.LogError("------------------------------------------------");
            }
        }

        protected virtual float CalculateDamage(DamageInfo damageInfo)
        {
            return damageComponent.GetFinalReceivedDamageAmount(damageInfo) * simpleDamageFactor;
        }

        public void Damage(DamageInfo damageInfo)
        {
            if (!IsServer) return;
            var trueDamageAmount = CalculateDamage(damageInfo);
            enemy.Damage(trueDamageAmount, damageInfo.dealer);
            enemy.OnTargetPlayerChangeRequired(damageInfo.dealer);
        }

        public float getCurrentHealth() => enemy.currentHealth.Value;
    }
}
