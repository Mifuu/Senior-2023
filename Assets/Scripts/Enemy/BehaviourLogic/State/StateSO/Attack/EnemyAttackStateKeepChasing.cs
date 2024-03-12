using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "AttackStateStillChasing", menuName = "Enemy/Enemy State/Attack State/Attack while running")]
    public class EnemyAttackStateKeepChasing : EnemyAttackStateWaitEndEvent
    {
        [Header("Chase while attack attribute")]
        [SerializeField] private float chaseSpeed = 10.0f;

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            transform.Translate(Vector3.forward * (chaseSpeed * Time.deltaTime));
        }
    }
}
