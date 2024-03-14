using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Chase Flying", menuName = "Enemy/Enemy State/Chase State/Chase Flying")]
    public class EnemyChaseFlying : EnemyChaseSOBase
    {
        EnemyLineOfSightCheck lineOfSightCheck;
        [SerializeField] private float chaseCooldownInterval = 5.0f;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            lineOfSightCheck = enemy.gameObject.GetComponentInChildren<EnemyLineOfSightCheck>();
            if (lineOfSightCheck == null) Debug.LogError("Line of Sight Check not found in chase state");
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(PerformFlyChase());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(PerformFlyChase());
        }

        private IEnumerator PerformFlyChase()
        {
            while (!lineOfSightCheck.IsPlayerInLineOfSight(enemy.targetPlayer))
            {
                // Set the destination to the player (Navmesh Agent)
                yield return new WaitForSeconds(chaseCooldownInterval);
            }

            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}
