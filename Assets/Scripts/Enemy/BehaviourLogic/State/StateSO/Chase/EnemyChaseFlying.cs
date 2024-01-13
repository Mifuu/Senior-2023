using System.Collections;
using System.Collections.Generic;
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
            enemy.PerformCoroutine(PerformFlyChase());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.PerformStopCoroutine(PerformFlyChase());
        }

        private IEnumerator PerformFlyChase()
        {
            while (!lineOfSightCheck.IsPlayerInLineOfSight())
            {
                // Set the destination to the player (Navmesh Agent)
                yield return new WaitForSeconds(chaseCooldownInterval);
            }

            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}
