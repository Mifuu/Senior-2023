using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemySpawnManager : NetworkBehaviour
    {
        public static EnemySpawnManager Singleton { get; private set; }
        public NetworkVariable<bool> isSpawning = new NetworkVariable<bool>(false);
        private List<GameObject> enemyPrefabList;
        [SerializeField] private GameObject chooseEnemyToSpawn;
        [SerializeField] private int spawnNumber = 1;

        public void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Singleton = this;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enemyPrefabList = NetworkObjectPool.Singleton.GetPrefabList();
            isSpawning.OnValueChanged += (prev, current) => ChangeSpawnState(current);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isSpawning.OnValueChanged -= (prev, current) => ChangeSpawnState(current);
        }

        private void ChangeSpawnState(bool current)
        {
            if (current)
            {
                StartCoroutine(SequentialSpawnCoroutine());
            }
            else
            {
                StopCoroutine(SequentialSpawnCoroutine());
            }
        }

        public void SpawnEnemy(GameObject enemyPrefab)
        {
            SpawnEnemy(enemyPrefab, GetRandomPosition());
        }

        public void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(enemyPrefab.transform.position, out hit, 1000f, NavMesh.AllAreas))
            {
                var enemy = NetworkObjectPool.Singleton.GetNetworkObject(enemyPrefab, hit.position, Quaternion.identity);
                var navmeshagent = enemy.GetComponent<NavMeshAgent>();
                navmeshagent.enabled = false;
                navmeshagent.enabled = true;
                enemy.Spawn();
            }
        }

        public void SpawnRandomEnemy()
        {
            var randomEnemyIndex = Random.Range(0, enemyPrefabList.Count);
            SpawnEnemy(enemyPrefabList[randomEnemyIndex]);
        }

        public void SpawnSelectedEnemy()
        {
            if (chooseEnemyToSpawn == null)
            {
                Debug.LogError("No Enemy Prefab is selected, Cannot spawn");
                return;
            }

            int spawnCount = 0;
            while (spawnCount < spawnNumber)
            {
                Debug.Log("Spawning " + chooseEnemyToSpawn);
                SpawnEnemy(chooseEnemyToSpawn);
                spawnCount++;
            }
        }

        private Vector3 GetRandomPosition()
        {
            return new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
        }

        public IEnumerator SequentialSpawnCoroutine()
        {
            while (isSpawning.Value)
            {
                SpawnRandomEnemy();
                yield return new WaitForSeconds(5.0f);
            }
        }

        public void TestGroupSpawn(int spawnAmount)
        {
            enemyPrefabList.ForEach((enemyPrefab) =>
            {
                if (enemyPrefab.GetComponent<EnemyBase>() == null)
                {
                    Debug.Log(enemyPrefab + " is not an enemy");
                    return;
                }
                int count = 0;
                while (count <= spawnAmount)
                {
                    SpawnEnemy(enemyPrefab);
                    count++;
                }
            });
        }
    }
}
