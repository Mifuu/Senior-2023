using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "WaitEvent", menuName = "Enemy/Enemy State/Attack State/Wait Ends")]
    public class EnemyAttackStateWaitEndEvent : EnemyAttackSOBase
    {
        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            selectedNextAttack.PerformAttack();
            selectedNextAttack.OnAttackEnds += OnAttackEnds;
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            selectedNextAttack.OnAttackEnds -= OnAttackEnds;
        }

        private void OnAttackEnds()
        {
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
