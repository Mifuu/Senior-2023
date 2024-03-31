using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public class OrchestratedSpawnManager : PersonalEnemySpawnManager
    {
        [Header("Orchestrator")]
        [SerializeField] private bool spawnImmediately;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            List<GameObject> newEnemyPrefabList = new List<GameObject>();
            foreach (var enemy in enemyPrefabList)
            {
                if (enemy.TryGetComponent<EnemyStateHijackable>(out EnemyStateHijackable hijackable))
                {
                    newEnemyPrefabList.Add(enemy);
                    continue;
                }
                Debug.LogError(enemy + "'s State can not be hijacked");
            }

            enemyPrefabList = newEnemyPrefabList;
            if (spawnImmediately) Spawn();
        }
    }
}
