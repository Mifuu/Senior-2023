using Unity.Netcode;
using UnityEngine;
namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyStateHijackable : NetworkBehaviour
    {
        EnemyBase enemy;
        GameObject enemyGameObject;

        private bool isHijacked = false;
        private GameObject currentHijacker = null;

        EnemyIdleSOBase idleBase;
        EnemyChaseSOBase chaseBase;
        EnemyAttackSOBase attackBase;
        EnemyKnockbackSOBase knockbackBase;

        public void Awake()
        {
            enemy = GetComponent<EnemyBase>();
            enemyGameObject = enemy.gameObject;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ForceUnhijack();
        }

        // This function was written when i'm drunk af, probably dont use it
        public void HijackState(
                GameObject hijacker,
                EnemyIdleSOBase idleBase = null,
                EnemyChaseSOBase chaseBase = null,
                EnemyAttackSOBase attackBase = null,
                EnemyKnockbackSOBase knockbackBase = null
            )
        {
            if (isHijacked)
            {
                Debug.LogError("Can not hijacked the same enemy twice");
                return;
            }

            idleBase = enemy.EnemyIdleBaseInstance;
            chaseBase = enemy.EnemyChaseBaseInstance;
            attackBase = enemy.EnemyAttackBaseInstance;
            knockbackBase = enemy.EnemyKnockbackBaseInstance;

            if (idleBase != null)
            {
                enemy.EnemyIdleBaseInstance = Instantiate(idleBase);
                enemy.EnemyIdleBaseInstance.Initialize(enemyGameObject, enemy);
                isHijacked = true;
            }

            if (chaseBase != null)
            {
                enemy.EnemyChaseBaseInstance = Instantiate(chaseBase);
                enemy.EnemyChaseBaseInstance.Initialize(enemyGameObject, enemy);
                isHijacked = true;
            }

            if (attackBase != null)
            {
                enemy.EnemyAttackBaseInstance = Instantiate(attackBase);
                enemy.EnemyAttackBaseInstance.Initialize(enemyGameObject, enemy);
                isHijacked = true;
            }

            if (knockbackBase != null)
            {
                enemy.EnemyKnockbackBaseInstance = Instantiate(knockbackBase);
                enemy.EnemyKnockbackBaseInstance.Initialize(enemyGameObject, enemy);
                isHijacked = true;
            }
        }

        public EnemyAttackSOBase HijackAttackStateInitializeOnly(EnemyAttackSOBase attackBase)
        {
            this.attackBase = enemy.EnemyAttackBaseInstance;
            enemy.EnemyAttackBaseInstance = attackBase;
            if (attackBase.allAttack.Count == 0) Debug.LogError("Detect that attack count equals 0, will cause index overflow problem");
            enemy.EnemyAttackBaseInstance.Initialize(enemyGameObject, enemy);
            foreach (var attacks in enemy.EnemyAttackBaseInstance.allAttack)
            {
                attacks.Initialize(enemy.targetPlayer, gameObject);
            }
            isHijacked = true;
            return enemy.EnemyAttackBaseInstance;
        }

        public EnemyAttackSOBase HijackAttackStateInstantiateOnly(EnemyAttackSOBase attackBase)
        {
            this.attackBase = enemy.EnemyAttackBaseInstance;
            enemy.EnemyAttackBaseInstance = Instantiate(attackBase);
            for (int i = 0; i < this.attackBase.allAttack.Count; i++)
            {
                enemy.EnemyAttackBaseInstance.allAttack[i] = Instantiate(enemy.EnemyAttackBaseInstance.allAttack[i]);
            }
            isHijacked = true;
            return enemy.EnemyAttackBaseInstance;
        }

        public void UnHijackState(GameObject hijacker)
        {
            if (hijacker != currentHijacker)
            {
                Debug.LogError("Only the hijacker could unhijack the state");
                return;
            }

            if (isHijacked)
            {
                ForceUnhijack();
            }
        }

        public void ForceUnhijack()
        {
            // EX. Used when the enemy die
            enemy.EnemyIdleBaseInstance = idleBase;
            enemy.EnemyChaseBaseInstance = chaseBase;
            enemy.EnemyAttackBaseInstance = attackBase;
            enemy.EnemyKnockbackBaseInstance = knockbackBase;
            isHijacked = false;
        }
    }
}
