namespace Enemy
{
    public class EnemyReturnState : EnemyState
    {
        public EnemyReturnState(EnemyBase enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
        {
            stateId = EnemyStateMachine.AvailableEnemyState.Return;
        }

        public override void AnimationTrigger(int triggerType)
        {
            base.AnimationTrigger(triggerType);
            enemy.EnemyReturnBaseInstance.DoAnimationTriggerEventLogic();
        }

        public override void EnterState()
        {
            base.EnterState();
            enemy.EnemyReturnBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            base.ExitState();
            enemy.EnemyReturnBaseInstance.DoExitLogic();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
            enemy.EnemyReturnBaseInstance.DoFrameUpdateLogic();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            enemy.EnemyReturnBaseInstance.DoPhysicsUpdateLogic();
        }
    }
}
