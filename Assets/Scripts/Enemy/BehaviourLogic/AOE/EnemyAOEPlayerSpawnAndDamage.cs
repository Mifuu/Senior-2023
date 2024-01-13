using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAOEPlayerSpawnAndDamage : EnemyAOEPlayerSpawnAndActivate
    {
        public override void ActivateEffect()
        {
            foreach (var players in areaOfEffectTrigger.PlayerWithinTrigger)
            {
                // Grab the damageable or damagecalculatable and do the damage
                Debug.Log("Doing damage to: " + players);
                throw new System.NotImplementedException("Implement EnemyAOEPlayerSpawnAndDamage");
            }
        }

        public override void CancelEffect()
        {
            throw new System.NotImplementedException("Implement EnemyAOEPlayerSpawnAndDamage");
        }
    }
}
