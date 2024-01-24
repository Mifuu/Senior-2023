using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Launch Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Launch")]
    public class LaunchAttack : EnemyAttack
    {
        [Header("Launch Attack Attribute")]
        [SerializeField] private float waitTime = 3.0f;
        [SerializeField] private float launchSpeed = 1000f;
        [SerializeField] private float preAttackWaitTime = 0.5f;
        [SerializeField] private float damagableTime = 1.0f;
        private bool canDamage = false;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            // TODO: Recheck the name, maybe hitbox needs to be inherited somewhere
            var hitbox = enemy.transform.Find("HitBox")?.gameObject.GetComponentInChildren<EnemyTriggerCheck>();
            if (hitbox == null)
            {
                Debug.LogError("Enemy has no hitbox");
                return;
            }

            hitbox.OnHitboxTriggerEnter += DamagePlayer;
        }

        public override void PerformAttack()
        {
            if (!enemy.IsServer) return;
            enemy.StartCoroutine(Launch());
        }

        private IEnumerator Launch()
        {
            yield return new WaitForSeconds(preAttackWaitTime);

            canDamage = true;
            enemy.rigidBody.AddForce(enemy.transform.forward * launchSpeed);
            yield return new WaitForSeconds(waitTime);
            canDamage = false;
            EmitAttackEndsEvent();

            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        public void DamagePlayer(Collider collider)
        {
            if (!canDamage)
            {
                return;
            }

            var info = Damage(collider.GetComponent<IDamageCalculatable>());
        }
    }
}
