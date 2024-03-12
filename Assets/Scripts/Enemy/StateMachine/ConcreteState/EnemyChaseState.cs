using UnityEngine;

namespace Enemy
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            stateId = EnemyStateMachine.AvailableEnemyState.Chase;
        }

        public override void AnimationTrigger(int triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyChaseBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            enemy.EnemyChaseBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
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
