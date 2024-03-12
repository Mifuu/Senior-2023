using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "AttackStateStillChasing", menuName = "Enemy/Enemy State/Attack State/Attack while running")]
    public class EnemyAttackStateKeepChasing : EnemyAttackStateWaitEndEvent
    {
        [Header("Chase while attack attribute")]
        [SerializeField] private float chaseSpeed = 10.0f;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.navMeshAgent.isStopped = true;
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.navMeshAgent.isStopped = false;
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            transform.LookAt(enemy.targetPlayer.transform);
            transform.Translate(Vector3.forward * (chaseSpeed * Time.deltaTime));
        }
    }
}
