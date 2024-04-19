using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "ReturnToOrigin", menuName = "Enemy/Enemy State/Return State/Origin")]
    public class EnemyReturnBackToOrigin : EnemyReturnSOBase
    {
        [Header("Checker")]
        [SerializeField] private float checkInterval = 1.0f;
        [SerializeField] private float stoppingDistance = 5f;
        private IEnumerator checkCoroutine;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            checkCoroutine = CheckForDistance();
            enemy.StartCoroutine(checkCoroutine);
            enemy.navMeshAgent.SetDestination(enemy.pointOfOrigin);
            enemy.animator.SetTrigger(chaseTrigger);
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(checkCoroutine);
        }

        private IEnumerator CheckForDistance()
        {
            while (true)
            {
                if ((enemy.targetPlayer.transform.position - enemy.pointOfOrigin).sqrMagnitude < enemy.maxDistanceSqrFromSpawnPoint)
                    enemy.StateMachine.ChangeState(enemy.ChaseState);
                else
                {
                    if (HasEnemyReachItsCurrentDestination)
                        enemy.Die(null);
                }
                yield return new WaitForSeconds(checkInterval);
            }
        }

        public bool HasEnemyReachItsCurrentDestination
        {
            get
            {
                var temp = false;
                if (!enemy.navMeshAgent.pathPending)
                {
                    if (enemy.navMeshAgent.remainingDistance <= stoppingDistance)
                    {
                        if (!enemy.navMeshAgent.hasPath || enemy.navMeshAgent.velocity.sqrMagnitude <= 0.1f)
                        {
                            temp = true;
                        }
                    }
                }
                return temp;
            }
        }
    }
}
