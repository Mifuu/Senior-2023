using System.Collections;
using UnityEngine;

namespace Enemy
{
    // NOTE: To Perform multiple melee attack, uses state to chain those attack
    [CreateAssetMenu(menuName = "Enemy/Enemy Logic/Attack Pattern/Melee", fileName = "Melee")]
    public class MeleeAttack : EnemyAttack
    {
        [Header("Melee Attack Attribute")]
        [SerializeField] private float waitTime = 1.0f;
        [SerializeField] private float timeTillCheck = 1.0f;
        [SerializeField] private float checkTime = 4.0f;
        [SerializeField] private float checkInterval = .5f; // Interval between each check in the checking period
        [SerializeField] private float cooldownTime = 2.0f;

        private EnemyWithinTriggerCheck hitbox;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component = null)
        {
            base.Initialize(targetPlayer, enemyGameObject, component);
            hitbox = enemy.transform.Find("DamageBox")?.GetComponent<EnemyWithinTriggerCheck>();
            if (hitbox == null) Debug.LogError("Enemy has no Damagebox");
        }

        public override void PerformAttack()
        {
            if (!enemy.IsServer) return;
            enemy.StartCoroutine(PerformAttackCoroutine());
        }

        private IEnumerator PerformAttackCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            PreAttack();
            yield return new WaitForSeconds(timeTillCheck);
            yield return PerformCheck();
            PostAttack();
            yield return new WaitForSeconds(cooldownTime);
            ResetProcessedDamageable();
            EmitAttackEndsEvent();
        }

        private IEnumerator PerformCheck()
        {
            int checkAmount = (int)(checkTime / checkInterval);
            for (int i = 0; i < checkAmount; i++)
            {
                foreach (GameObject player in hitbox.PlayerWithinTrigger)
                {
                    var info = Damage(player.GetComponentInChildren<IDamageCalculatable>());
                }
                yield return new WaitForSeconds(checkInterval);
            }
        }

        protected virtual void PreAttack() { }
        protected virtual void PostAttack() { }
    }
}
