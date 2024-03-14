using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Swarm Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Swarm")]
    public class SwarmAttack : EnemyAttack
    {
        PersonalEnemySpawnManager spawnManager;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            spawnManager = enemy.gameObject.GetComponent<PersonalEnemySpawnManager>();
        }

        public override void PerformAttack()
        {
            spawnManager.Spawn();
            spawnManager.OnAllEnemyDies += EndAttack;
            spawnManager.OnAllEnemyDies += LogAllEnemiesDies;
        }

        public void EndAttack()
        {
            spawnManager.OnAllEnemyDies -= EndAttack;
            EmitAttackEndsEvent();
        }
        
        private void LogAllEnemiesDies()
        {
            spawnManager.OnAllEnemyDies -= LogAllEnemiesDies;
        }
    }
}
