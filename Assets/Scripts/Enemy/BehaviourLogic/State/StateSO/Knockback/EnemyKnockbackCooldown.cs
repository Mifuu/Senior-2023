using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Cooldown", menuName = "Enemy/Enemy State/Knockback State/Cooldown")]
    public class EnemyKnockbackCooldown : EnemyKnockbackSOBase
    {
        [SerializeField] float cooldownTime = 5.0f;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.PerformCoroutine(Cooldown());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.PerformStopCoroutine(Cooldown());
        }

        public IEnumerator Cooldown()
        {
            // TODO: Maybe define more of the knockback logic
            yield return new WaitForSeconds(cooldownTime);
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}
