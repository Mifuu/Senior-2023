using UnityEngine;

namespace Enemy
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
        }

        public override void AnimationTrigger(EnemyBase.AnimationTriggerType triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyChaseBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("Entering Chase State");
            enemy.EnemyChaseBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("Exiting Chase State");
            enemy.EnemyChaseBaseInstance.DoExitLogic();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            enemy.EnemyChaseBaseInstance.DoFrameUpdateLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            enemy.EnemyChaseBaseInstance.DoPhysicsUpdateLogic();
        }
    }
}
