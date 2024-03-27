using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Swarm Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Swarm")]
    public class SwarmAttack : EnemyAttack
    {
        PersonalEnemySpawnManager spawnManager;
        [SerializeField] private string spawnManagerId;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component = null)
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
            spawnManager.Spawn();
            spawnManager.OnAllEnemyDies += EndAttack;
        }

        public void EndAttack()
        {
            spawnManager.OnAllEnemyDies -= EndAttack;
            EmitAttackEndsEvent();
        }
    }
}
