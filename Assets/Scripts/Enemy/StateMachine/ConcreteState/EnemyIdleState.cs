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
            enemy.EnemyIdleBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
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
