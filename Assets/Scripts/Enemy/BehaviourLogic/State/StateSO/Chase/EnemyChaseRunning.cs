using UnityEngine;
using System.Collections;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Chase Running", menuName = "Enemy/Enemy State/Chase State/Chase Running")]
    public class EnemyChaseRunning : EnemyChaseSOBase
    {
        [SerializeField] private float chaseSpeed = 5.0f;
        [SerializeField] private float targetCheckInterval = 1f;
        private EnemyWithinTriggerCheck strikingDistanceCheck;
        private bool isMoving = false;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            strikingDistanceCheck = enemy.transform.Find("StrikingDistance")?.GetComponent<EnemyWithinTriggerCheck>();
            if (strikingDistanceCheck == null) Debug.LogError("Enemy has no Striking Distance Check");
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            if (!enemy.IsServer) return;
            isMoving = true;
            enemy.StartCoroutine(Move());
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            if (!enemy.IsServer) return;

            transform.LookAt(enemy.targetPlayer.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            if (strikingDistanceCheck.PlayerWithinTrigger.Count != 0)
            {
                enemy.StateMachine.ChangeState(enemy.AttackState);
            }
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(Move());
        }

        public IEnumerator Move()
        {
            while (isMoving)
            {
                enemy.navMeshAgent?.SetDestination(enemy.targetPlayer.transform.position);
                yield return new WaitForSeconds(targetCheckInterval);
            }
        }
    }
}
