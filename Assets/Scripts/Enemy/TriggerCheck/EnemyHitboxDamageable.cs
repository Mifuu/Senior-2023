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
            enemy = GetComponentInParent<EnemyBase>();
            if (enemy == null) Debug.LogError("EnemyBase component not found");
        }

        // TODO: Temporary Damage Calculator
        public virtual float CalculateDamage(DamageInfo damageInfo)
        {
            return simpleDamageFactor * damageInfo.amount;
        }

        public void Damage(DamageInfo damageInfo)
        {
            var trueDamageAmount = CalculateDamage(damageInfo);
            Debug.Log("Damaging enemy: " + trueDamageAmount);
            enemy.Damage(trueDamageAmount);
        }

        public float getCurrentHealth()
        {
            return enemy.currentHealth.Value;
        }

        // Weakpoint rules 
        // 1. No Weakpoint, No Elemental hit = 1x multiplier
        // 2. Hitting with correct weak elemental or hit the weakpoint = 1.5x multiplier
        // 3. Hitting the enemy on the weakpoint with the correct element results in a knockback and high multiplier = 3x
        // 4. Some enemy could be knockback with the corrent gun
        //
        // Class EnemyHitBox, (Contains IDamageCalculatable interface which could be grab easily)
        // - Can check for collision => must expose the collision event as well
        // - Collision is used for API only, damage must be given by the player doing the damage via dodamage function
        // - Has method to call for DMG, param: DamageInfo
        // - Can calculate the damage then dealth it to the enemy base class
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
