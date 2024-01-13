using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Stand Still", menuName = "Enemy/Enemy State/Idle State/Stand Still")]
    public class EnemyIdleStandStill : EnemyIdleSOBase
    {
        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            // This state is a placeholder, move on to the next state
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}

