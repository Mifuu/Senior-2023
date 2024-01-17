using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    public class EnemyAOEPlayerSpawnAndDamage : EnemyAOEPlayerSpawnAndActivate
    {
        [SerializeField] private float baseDamageAmount = 5.0f;

        public override void ActivateEffect()
        {
            foreach (var players in areaOfEffectTrigger.PlayerWithinTrigger)
            {
                DamageInfo info = new DamageInfo(enemy.gameObject, baseDamageAmount);
                var damager = players.GetComponent<IDamageCalculatable>();
                if (damager == null)
                {
                    return;
                }

                damager.Damage(info);
                Debug.Log("Damaging: " + info.amount + ", @" + DateTime.Now.ToString());
            }
        }

        public override void CancelEffect()
        {
            Debug.Log("Cancelling Effect");
            EmitAOEEndsEvent();
            // throw new System.NotImplementedException("Implement EnemyAOEPlayerSpawnAndDamage");
        }
    }
}
