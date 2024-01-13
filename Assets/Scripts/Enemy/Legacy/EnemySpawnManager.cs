using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace LegacyEnemy
{
    [Obsolete("Deprecated: This codes belong to old enemy codebase")]
    public class EnemySpawnManager : Singleton<EnemySpawnManager>
    {
        [SerializeField] EnemyScriptableObject[] enemyTypes;
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] NetworkVariable<bool> isSpawning = new NetworkVariable<bool>(false);

        public bool IsSpawning => isSpawning.Value;

        public void SetIsSpawn(bool value)
        {
            isSpawning.Value = value;
        }

        public void SetIsSpawn(Func<bool, bool> func)
        {
            bool newValue = func(isSpawning.Value);
            isSpawning.Value = newValue;
        }

        public EnemyScriptableObject GetEnemyConfigById(int id)
        {
            if (enemyTypes.Length == 0 || id >= enemyTypes.Length || id < 0)
            {
                return null;
            }
            return enemyTypes[id];
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }

            isSpawning.OnValueChanged += (_, current) =>
            {
                if (current)
                {
                    StartCoroutine(SpawnEnemy());
                }
                else
                {
                    StopCoroutine(SpawnEnemy());
                }
            };
        }

        IEnumerator SpawnEnemy()
        {
            while (isSpawning.Value)
            {
                if (enemyTypes.Length == 0)
                {
                    yield return new WaitForSeconds(5.0f);
                    continue;
                }
                Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-5, 5), 20, UnityEngine.Random.Range(-5, 5));
                GameObject enemyGameObject = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
                if (Physics.Raycast(enemyGameObject.transform.position, Vector3.down, out RaycastHit hit))
                {
                    enemyGameObject.transform.position -= new Vector3(0, -1 + hit.distance - enemyGameObject.GetComponent<Collider>().bounds.size.z, 0);
                }

                NetworkObject enemyNetwork = enemyGameObject.GetComponent<NetworkObject>();
                enemyNetwork.Spawn();
                Enemy enemy = enemyGameObject.GetComponent<Enemy>();
                enemy.InitializeConfigId(UnityEngine.Random.Range(0, enemyTypes.Length));
                yield return new WaitForSeconds(5.0f);
            }
        }
    }
}
