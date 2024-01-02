using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemySpawnManager : NetworkBehaviour
    {
        public EnemySpawnManager Singleton { get; private set; }
        public NetworkVariable<bool> isSpawning = new NetworkVariable<bool>(false);
        private List<GameObject> enemyPrefabList;

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
            var enemy = NetworkObjectPool.Singleton.GetNetworkObject(enemyPrefab, GetRandomPosition(), Quaternion.identity);
            enemy.Spawn();
        }

        public void SpawnRandomEnemy()
        {
            var randomEnemyIndex = Random.Range(0, enemyPrefabList.Count);
            SpawnEnemy(enemyPrefabList[randomEnemyIndex]);
        }

        private Vector3 GetRandomPosition()
        {
            return new Vector3(Random.Range(-5f, 5f), 5, Random.Range(-5f, 5f));
        }

        public IEnumerator SequentialSpawnCoroutine()
        {
            while (isSpawning.Value)
            {
                SpawnRandomEnemy();
                yield return new WaitForSeconds(5.0f);
            }
        }
    }
}
