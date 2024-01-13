using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Launch Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Launch")]
    public class LaunchAttack : EnemyAttack
    {
        [SerializeField] private float waitTime = 3.0f;
        [SerializeField] private float launchSpeed = 1000f;
        [SerializeField] private float preAttackWaitTime = 0.5f;
        [SerializeField] private float damagableTime = 1.0f;
        private bool canDamage = false;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            // TODO: Recheck the name, maybe hitbox needs to be inherited somewhere
            var hitbox = enemy.transform.Find("HitBox")?.gameObject.GetComponentInChildren<EnemyHitbox>();
            if (hitbox == null)
            {
                Debug.LogError("Enemy has no hitbox");
                return;
            }

            hitbox.OnHitboxTriggerEnter += DamagePlayer;
        }

        public override void PerformAttack()
        {
            enemy.PerformCoroutine(Launch());
        }

        private IEnumerator Launch()
        {
            yield return new WaitForSeconds(preAttackWaitTime);
            canDamage = true;
            enemy.rigidBody.AddForce(enemy.transform.forward * launchSpeed);
            yield return new WaitForSeconds(waitTime);
            canDamage = false;
            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        // TODO: Has trigger enter subscription to perform damage to the player
        public void DamagePlayer(Collider collider)
        {
            if (canDamage)
            {
                Debug.Log("Player should be damaged here");
            }
            else
            {
                Debug.Log("Collision to enemy hitbox");
            }
            throw new System.NotImplementedException("Please implement the DamagePlayer function in LaunchAttack.cs");
        }
    }
}
