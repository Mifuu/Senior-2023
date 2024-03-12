namespace Enemy
{
    public class EnemyState
    {
        protected EnemyBase enemy;
        protected EnemyStateMachine enemyStateMachine;
        public EnemyStateMachine.AvailableEnemyState stateId;

        public EnemyState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            this.enemy = enemy;
            this.enemyStateMachine = enemyStateMachine;
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void FrameUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void AnimationTrigger(int triggerType) { }
    }
}
