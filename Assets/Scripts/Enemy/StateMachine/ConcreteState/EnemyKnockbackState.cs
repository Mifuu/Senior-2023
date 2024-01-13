using UnityEngine;

namespace Enemy
{
    public class EnemyKnockbackState : EnemyState
    {
        public EnemyKnockbackState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void AnimationTrigger(EnemyBase.AnimationTriggerType triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyKnockbackBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("Entering Knockback State");
            enemy.EnemyKnockbackBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("Exiting Knockback State");
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
