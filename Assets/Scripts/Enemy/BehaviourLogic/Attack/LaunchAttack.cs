using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Launch Attack", menuName = "Enemy Logic/Attack Pattern/Launch")]
    public class LaunchAttack : EnemyAttack
    {
        [SerializeField] private float waitTime = 3.0f;
        [SerializeField] private float launchSpeed = 1000f;
        [SerializeField] private float preAttackWaitTime = 0.5f;

        public override void PerformAttack()
        {
            enemy.PerformCoroutine(Launch());
        }

        private IEnumerator Launch()
        {
            yield return new WaitForSeconds(preAttackWaitTime);
            enemy.rigidBody.AddForce(enemy.transform.forward * launchSpeed);
            yield return new WaitForSeconds(waitTime);
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }
    }
}
