using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyState
    {
        protected EnemyBase enemy;
        protected EnemyStateMachine enemyStateMachine;
        public string stateId;

        public EnemyState(EnemyBase enemy, EnemyStateMachine enemyStateMachine)
        {
            this.enemy = enemy;
            this.enemyStateMachine = enemyStateMachine;
        }

        public virtual void EnterState() { Debug.Log("State Changed: " + stateId); }
        public virtual void ExitState() { }
        public virtual void FrameUpdate() { }
        public virtual void PhysicsUpdate() { }
        public virtual void AnimationTrigger(EnemyBase.AnimationTriggerType triggerType) { }
    }
}
