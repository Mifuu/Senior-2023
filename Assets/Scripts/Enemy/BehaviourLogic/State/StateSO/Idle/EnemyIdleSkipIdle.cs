using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Skip Idle", menuName = "Enemy/Enemy State/Idle State/Skip Idle")]
    public class EnemyIdleSkipIdle : EnemyIdleSOBase
    {
        [SerializeField] private float cooldownTime;

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown() 
        {
            yield return new WaitForSeconds(cooldownTime);
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}
