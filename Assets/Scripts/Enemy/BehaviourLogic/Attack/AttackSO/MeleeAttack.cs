using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    // NOTE: To Perform multiple melee attack, uses state to chain those attack
    [CreateAssetMenu(menuName = "Enemy/Enemy Logic/Attack Pattern/Melee", fileName = "Melee")]
    public class MeleeAttack : EnemyAttack
    {
        // TODO: Check the case where there are multiple hitboxes in the enemy prefab
        [Header("Melee Attack Attribute")]
        [SerializeField] private float waitTime = 1.0f;
        [SerializeField] private float timeTillCheck = 1.0f;
        [SerializeField] private float checkTime = 4.0f;
        [SerializeField] private float checkInterval = .5f; // Interval between each check in the checking period
        [SerializeField] private float cooldownTime = 2.0f;

        private EnemyWithinTriggerCheck hitbox;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
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
            EmitAttackEndsEvent();
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
                        var info = Damage(player.GetComponent<IDamageCalculatable>());
                    }
                }
                yield return new WaitForSeconds(checkInterval);
            }
        }

        protected virtual void PreAttack() { }
        protected virtual void PostAttack() { }
    }
}
