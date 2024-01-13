using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Normal Attack", menuName = "Enemy/Enemy State/Attack State/Normal Attack")]
    public class EnemyAttackNormal : EnemyAttackSOBase
    {
        [SerializeField] float cooldownTime = 5.0f;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            if (allAttack.Count != 0)
            {
                allAttack[0].PerformAttack();
                enemy.PerformCoroutine(Cooldown());
                return;
            }
            Debug.LogError("Enemy Has No Attack");
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        public IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}
