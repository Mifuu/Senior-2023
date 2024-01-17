using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Stand Still", menuName = "Enemy/Enemy State/Idle State/Stand Still")]
    public class EnemyIdleStandStill : EnemyIdleSOBase
    {
        [Header("Stand Still Time")]
        [SerializeField] private float cooldownTime = 3.0f;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.PerformCoroutine(StandStill());
        }

        public IEnumerator StandStill()
        {
            yield return new WaitForSeconds(cooldownTime);
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            yield break;
        }
    }
}

