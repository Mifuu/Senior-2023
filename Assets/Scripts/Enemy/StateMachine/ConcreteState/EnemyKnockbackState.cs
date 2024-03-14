using UnityEngine;

namespace Enemy
{
    public class EnemyKnockbackState : EnemyState
    {
        public EnemyKnockbackState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            stateId = EnemyStateMachine.AvailableEnemyState.Knockback;
        }

        public override void AnimationTrigger(int triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyKnockbackBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            enemy.EnemyKnockbackBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            enemy.EnemyKnockbackBaseInstance.DoExitLogic();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            enemy.EnemyKnockbackBaseInstance.DoFrameUpdateLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            enemy.EnemyKnockbackBaseInstance.DoPhysicsUpdateLogic();
        }
    }
}
