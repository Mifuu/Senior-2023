using UnityEngine;

namespace Enemy
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            stateId = EnemyStateMachine.AvailableEnemyState.Attack;
        }

        public override void AnimationTrigger(int triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyAttackBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            enemy.EnemyAttackBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
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
