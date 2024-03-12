using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Launch Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Launch")]
    public class LaunchAttack : EnemyAttack
    {
        [Header("Launch Attack Attribute")]
        [SerializeField] private float launchSpeed = 10f;
        [SerializeField] private float preAttackWaitTime = 0.5f;
        [SerializeField] private float damagableTime = 1.0f;
        private bool canDamage = false;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            // Make sure to recheck the name and check that the EnemyTriggerCheck is in there
            var hitbox = enemy.transform.Find("HitBox")?.gameObject.GetComponentInChildren<EnemyTriggerCheck>();
            if (hitbox == null)
            {
                Debug.LogError(enemy.gameObject + " Has No Damager Hitbox");
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
            enemy.animationEventEmitter.OnAttackAnimationEnds += EndingAttack;
            enemy.navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(preAttackWaitTime);

            enemy.transform.LookAt(enemy.targetPlayer.transform);
            canDamage = true;
            enemy.rigidBody.velocity = enemy.transform.forward * launchSpeed;
        }

        public void EndingAttack()
        {
            enemy.animationEventEmitter.OnAttackAnimationEnds -= EndingAttack;
            canDamage = false;
            enemy.rigidBody.velocity = Vector3.zero;
            EmitAttackEndsEvent();
            ResetProcessedDamageable();
        }

        public void DamagePlayer(Collider collider)
        {
            if (!canDamage) return;
            Damage(collider.GetComponentInChildren<IDamageCalculatable>());
        }
    }
}
