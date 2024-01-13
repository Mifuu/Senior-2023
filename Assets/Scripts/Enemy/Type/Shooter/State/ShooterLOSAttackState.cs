using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LineOfSightCheckAttack", menuName = "Enemy/Enemy State/Attack State/Line Of Sight Check")]
    public class ShooterLOSAttackState : EnemyAttackSOBase
    {
        [SerializeField] float afterAttackCoolDownTime = 5.0f;
        EnemyLineOfSightCheck lineOfSightCheck;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            lineOfSightCheck = enemy.GetComponentInChildren<EnemyLineOfSightCheck>();
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.PerformCoroutine(ShootAndCheckLineOfSightCoroutine());
            if (allAttack.Count == 0)
            {
                Debug.LogError("No Attack, Moving to Idle State");
                enemy.StateMachine.ChangeState(enemy.IdleState);
            }
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.PerformStopCoroutine(ShootAndCheckLineOfSightCoroutine());
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            enemy.transform.LookAt(playerTransform);
            enemy.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // Perform attack, Check line of sight, Repeat
        public IEnumerator ShootAndCheckLineOfSightCoroutine()
        {
            bool isContinueAttacking = true;
            while (isContinueAttacking)
            {
                allAttack[0].PerformAttack();
                yield return new WaitForSeconds(afterAttackCoolDownTime);
                if (!lineOfSightCheck.IsPlayerInLineOfSight())
                {
                    Debug.Log("Shooter checking LOS, State: Attack");
                    isContinueAttacking = false;
                    enemy.StateMachine.ChangeState(enemy.ChaseState);
                }
            }
        }
    }
}
