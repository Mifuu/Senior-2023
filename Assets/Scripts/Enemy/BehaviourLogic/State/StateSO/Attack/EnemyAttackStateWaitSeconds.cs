using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "WaitSeconds", menuName = "Enemy/Enemy State/Attack State/Wait Seconds")]
    public class EnemyAttackStateWaitSeconds : EnemyAttackSOBase
    {
        [SerializeField] private float timeBeforeExitState = 3.0f;
        [SerializeField] private float timeBeforeAttackBegin = 2.0f;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(PerformAttackCoroutine());
        }

        public virtual IEnumerator PerformAttackCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeAttackBegin);
            selectedNextAttack.PerformAttack();
            yield return new WaitForSeconds(timeBeforeExitState);
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            transform.LookAt(enemy.targetPlayer.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
