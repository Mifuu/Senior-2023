using UnityEngine;

namespace Enemy
{
    public class EnemyBasedShield : CountBasedShield
    {
        [Header("Spawn Manager Setup")]
        [SerializeField] private PersonalEnemySpawnManager spawnManager;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            spawnManager.currentAliveEnemy.OnValueChanged += SynchronizeCountWithEnemyAlive;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            spawnManager.currentAliveEnemy.OnValueChanged -= SynchronizeCountWithEnemyAlive;
        }

        private void SynchronizeCountWithEnemyAlive(int _, int current)
        {
            if (!IsServer) return;
            shieldHealthCount.Value = current;
        }
    }
}
