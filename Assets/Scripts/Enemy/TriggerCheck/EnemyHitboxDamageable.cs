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
            enemy.Damage(trueDamageAmount);
            enemy.OnTargetPlayerChangeRequired(damageInfo.dealer);
        }

        public float getCurrentHealth() => enemy.currentHealth.Value;

        // Weakpoint rules 
        // 1. No Weakpoint, No Elemental hit = 1x multiplier
        // 2. Hitting with correct weak elemental or hit the weakpoint = 1.5x multiplier
        // 3. Hitting the enemy on the weakpoint with the correct element results in a knockback and high multiplier = 3x
        // 4. Some enemy could be knockback with the corrent gun
        //
        // Damage IDEA:
        //  - Enemy only have the level
        //  - All the stat is defined in the scriptable object (EnemyStat), and will not changed
        //      - Stat could include:
        //          - BASE Damage Multiplier against all types of guns
        //          - BASE HP, ATK, MOVEMENTSPEED
        //  - Also when spawns HP, ATK, Movement speeds will be determined from the using the same way
        //  - Only when the damage needs to be calculated, should both the level and the stat be used
        //
        //  Factors that could affect damage
        //  - Current enemy stat + buff stat: Enemy nust have base stat and buffer multiplier
        //  - Elemental Type (nullable)
        //  - Is Weakpoint? 
        //  - Gun Type (nullable)
        //
        // ScriptableObject EnemyBaseStat  
        //  Container the weights for calculating for various types of attack 
        //  BASE HP, Base ATK, Base Movement Speed, Base DEF
        //  Emphasize the word "BASE"
        //  - Contains:
    }
}
