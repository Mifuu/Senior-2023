using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LineOfSightCheckAttack", menuName = "Enemy/Enemy State/Attack State/Line Of Sight Check")]
    public class ShooterLOSAttackState : EnemyAttackSOBase
    {
        [SerializeField] float afterAttackCoolDownTime = 1.0f;
        EnemyLineOfSightCheck lineOfSightCheck;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            lineOfSightCheck = enemy.GetComponentInChildren<EnemyLineOfSightCheck>();
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(ShootAndCheckLineOfSightCoroutine());
            if (allAttack.Count == 0)
            {
                Debug.LogError("No Attack, Moving to Idle State");
                enemy.StateMachine.ChangeState(enemy.IdleState);
            }
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(ShootAndCheckLineOfSightCoroutine());
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            enemy.transform.LookAt(enemy.targetPlayer.transform);
            enemy.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // Perform attack, Check line of sight, Repeat
        public IEnumerator ShootAndCheckLineOfSightCoroutine()
        {
            bool isContinueAttacking = true;
            while (isContinueAttacking)
            {
                selectedNextAttack.PerformAttack();
                yield return new WaitForSeconds(afterAttackCoolDownTime);
                if (!lineOfSightCheck.IsTargetPlayerInLineOfSight())
                {
                    isContinueAttacking = false;
                    enemy.animator.SetTrigger(enemy.finishedAttackingAnimationTrigger);
                    enemy.StateMachine.ChangeState(enemy.ChaseState);
                    yield break;
                }
            }
        }
    }
}
