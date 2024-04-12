using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Stand Still", menuName = "Enemy/Enemy State/Idle State/Stand Still")]
    public class EnemyIdleStandStill : EnemyIdleSOBase
    {
        [Header("Stand Still Time")]
        [SerializeField] private float cooldownTime = 3.0f;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(StandStill());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(StandStill());
        }

        public IEnumerator StandStill()
        {
            enemy.navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(cooldownTime);
            enemy.navMeshAgent.isStopped = false;
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            yield break;
        }
    }
}
