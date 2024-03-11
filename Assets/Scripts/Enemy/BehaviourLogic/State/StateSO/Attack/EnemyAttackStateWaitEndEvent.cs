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
            enemy.animator.SetTrigger(enemy.attackAnimationTrigger);
            selectedNextAttack.OnAttackEnds += OnAttackEnds;
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.animator.SetTrigger(enemy.finishedAttackingAnimationTrigger);
            selectedNextAttack.OnAttackEnds -= OnAttackEnds;
        }

        private void OnAttackEnds()
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            // TODO: Remove all the IsServer check from the State since client enemy no longer has the state machine
            if (!enemy.IsServer) return;
            transform.LookAt(enemy.targetPlayer.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
