using UnityEngine;

namespace Enemy
{
    public class OrchestratedSpawnManager : PersonalEnemySpawnManager
    {
        [Header("Orchestrator")]
        [SerializeField] private bool spawnImmediately;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            foreach (var enemy in enemyPrefabList)
            {
                if (!enemy.TryGetComponent<EnemyStateHijackable>(out EnemyStateHijackable hijackable))
                {
                    // enemyPrefabList.Remove(enemy); // BUG: List can not be modified while being iterated 
                    Debug.LogError(enemy + "'s State can not be hijacked");
                }
            }

            if (spawnImmediately)
            {
                Spawn();
            }
        }
    }
}
