using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyHitboxDamageable : NetworkBehaviour, IDamageCalculatable
    {
        private EnemyBase enemy;
        [SerializeField] private float simpleDamageFactor = 1.0f;

        public void Start()
        {
            enemy = GetComponentInParent<EnemyBase>(true);
            if (enemy == null)
            {
                Debug.LogError(gameObject + ": Enemy Base Class Not Found");
            }
        }

        // TODO: Temporary Damage Calculator
        protected virtual float CalculateDamage(DamageInfo damageInfo)
        {
            return simpleDamageFactor * damageInfo.amount;
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
