using UnityEngine;

namespace Enemy
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void AnimationTrigger(EnemyBase.AnimationTriggerType triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyIdleBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("Entering Idle State");
            enemy.EnemyIdleBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("Exiting Idle State");
            enemy.EnemyIdleBaseInstance.DoExitLogic();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            enemy.EnemyIdleBaseInstance.DoFrameUpdateLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            enemy.EnemyIdleBaseInstance.DoPhysicsUpdateLogic();
        }
    }
}
