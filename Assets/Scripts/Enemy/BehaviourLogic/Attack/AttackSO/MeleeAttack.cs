using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Logic/Attack Pattern/Melee", fileName = "Melee")]
    public class MeleeAttack : EnemyAttack
    {
        // TODO: Check the case where there are multiple hitboxes in the enemy prefab
        private EnemyHitbox hitbox;
        [SerializeField] private float waitTime = 4.0f;
        [SerializeField] private float timeTillCheck = 3.0f;
        [SerializeField] private float checkTime = 5.0f;
        [SerializeField] private float checkInterval = 1.0f; // Interval between each check in the checking period
        [SerializeField] private float cooldownTime = 3.0f;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            hitbox = enemy.transform.Find("DamageBox")?.GetComponent<EnemyHitbox>();
            if (hitbox == null) Debug.LogError("Enemy has no Damagebox");
        }

        public override void PerformAttack()
        {
            enemy.PerformCoroutine(PerformAttackCoroutine());
        }

        private IEnumerator PerformAttackCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            PreAttack();
            yield return new WaitForSeconds(timeTillCheck);
            yield return PerformCheck();
            PostAttack();
            yield return new WaitForSeconds(cooldownTime);
        }

        private IEnumerator PerformCheck()
        {
            int checkAmount = (int)(checkTime / checkInterval);
            for (int i = 0; i < checkAmount; i++)
            {
                if (hitbox.PlayerWithinTrigger.Count != 0)
                {
                    foreach (GameObject player in hitbox.PlayerWithinTrigger)
                    {
                        // Debug.Log("Performing check, #" + (i + 1) + ", time: " + System.DateTime.Now.ToString());
                        // Damage the Damageable 
                        var damager = player.GetComponent<IDamageCalculatable>();
                    }
                }
                yield return new WaitForSeconds(checkInterval);
            }
        }

        protected virtual void PreAttack() { }
        protected virtual void PostAttack() { }
    }
}
