using UnityEngine;

namespace Enemy
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void AnimationTrigger(EnemyBase.AnimationTriggerType triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyAttackBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("Entering Attack State");
            enemy.EnemyAttackBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("Exiting Attack State");
            enemy.EnemyAttackBaseInstance.DoExitLogic();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            enemy.EnemyAttackBaseInstance.DoFrameUpdateLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            enemy.EnemyAttackBaseInstance.DoPhysicsUpdateLogic();
        }
    }
}
