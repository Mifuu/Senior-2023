using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    // TODO: Implement the IDamageCalculatable 
    // Note: IDamagaCalculatable interface can stil be used by the player 
    public class EnemyHitboxDamageable : EnemyHitbox, IDamageCalculatable
    {
        // TODO: Add API for external to do damage
        // Should have param as DamageInfo
        public void PerformDamage()
        {
            // TODO: Enemy should have base stat
            // Calculate damage function and dealt the damage the enemy
            // (enemyLevel, baseDef[baseStat], DamageInfo[Info from the player]) => int
            throw new System.NotImplementedException();
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
        // Class/Struct EnemyDamageInfo
        // - Contains:
        //      Gun type?: Maybe an enum or something (Nullable: Normal ability may not have a gun type)
        //      Elemental type?: Also Maybe an enum or class or something else (Nullable: Attack may not have a gun type)
        //      BuffEffect?: Change the "buff factor (float, default = 1)" of certain stat (Maybe an enum) for "a period of time (float, unit = seconds)"
        //      Position: Vector3 for maybe knockback
        //      Either: 
        //          Damage Amount: float (Does not have to worry about damage calculation on the player side)
        //          or 
        //          Player Info, such as (Enemy have more control on how the damage is calculated on the enemy side)
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
        //
        // Interface Buffable
        // - Contains the extra multiplier for HP, ATK, MOVEMENT SPEED, DEF which defaults to 1 but changed on getting buffed by other character
        // - Must be able to apply buff for some known period of time
        // - IDEA: Contains some IEnumerator function that create a routine
    }
}
