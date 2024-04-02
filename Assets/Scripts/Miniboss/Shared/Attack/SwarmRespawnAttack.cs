using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "SwarmRespawnSomeAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/Swarm Respawn")]
    public class SwarmRespawnAttack : EnemyAttack
    {
        private PersonalEnemySpawnManager spawnManager;

        [SerializeField] private string spawnManagerId;
        [SerializeField] private int spawnAmount;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component)
        {
            base.Initialize(targetPlayer, enemyGameObject, component);
            var allSpawnManager = enemy.gameObject.GetComponentsInChildren<PersonalEnemySpawnManager>();
            foreach (var manager in allSpawnManager)
            {
                if (manager.UniqueId == spawnManagerId)
                    spawnManager = manager;
            }

            if (spawnManager == null)
                Debug.LogError($"Spawn Manager with id: {spawnManagerId} " + "is not found");
        }

        public override void PerformAttack()
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                spawnManager.SpawnRandomEnemyIntoVacantPosition();
                EmitAttackEndsEvent();
            }
        }
    }
}
