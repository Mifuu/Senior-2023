using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
using UnityEngine.AI;

namespace Enemy
{
    public class PersonalEnemySpawnManager : NetworkBehaviour
    {
        [Header("Component Config")]
        [SerializeField] public string UniqueId = "";

        [Header("Spawn Config")]
        private List<List<Vector3>> positionList;
        [SerializeField] private List<GameObject> enemyPrefabList;
        [SerializeField] private List<GameObject> SpawnGroupGameObject;

        [Tooltip("How many TYPE of enemy would be used in one spawn cycle")]
        [SerializeField] public int randomEnemyTypeAmount;
        [Tooltip("Try to have multiple types of enemies in one group")]
        [SerializeField] public bool stratify;
        [Tooltip("Completely randomize each group, ignore other setting")]
        [SerializeField] public bool groupMaxRandom;

        private List<EnemyBase> spawnedEnemyRef = new List<EnemyBase>();
        public NetworkVariable<int> currentAliveEnemy = new NetworkVariable<int>(0);
        private bool isInit = false;

        public event Action OnEnemySpawns;
        public event Action<EnemyBase> OnEnemyDies;
        public event Action OnAllEnemyDies;

        public void Awake()
        {
            if (UniqueId == "")
                Debug.LogError("Please specify the ID for PersonalEnemySpawnManager Component");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (SpawnGroupGameObject != null) SetSpawnGroupPosition(SpawnGroupGameObject);
        }

        public void SetSpawnGroupPosition(List<List<Vector3>> positionList)
        {
            this.positionList = positionList;
            isInit = true;
        }

        public void SetSpawnGroupPosition(List<GameObject> spawnGroup)
        {
            var posList = new List<List<Vector3>>();
            foreach (var group in spawnGroup)
            {
                var groupPosList = new List<Vector3>();
                foreach (Transform child in group.transform)
                {
                    groupPosList.Add(child.position);
                }
                posList.Add(groupPosList);
            }

            SetSpawnGroupPosition(posList);
        }

        public void SetEnemyList(List<GameObject> listOfEnemy)
        {
            this.enemyPrefabList = listOfEnemy;
        }

        public void Spawn()
        {
            System.Random rnd = new System.Random();

            var selectedEnemyList = enemyPrefabList.OrderBy(x => rnd.Next()).Take(randomEnemyTypeAmount);
            var spawnGroupGameObjectIndex = LoopingIndexGenerator(randomEnemyTypeAmount).GetEnumerator();

            foreach (var spawnGroupPosition in positionList)
            {
                if (stratify)
                {
                    for (int i = 0; i < spawnGroupPosition.Count; i++)
                    {
                        SpawnEnemyOntoNavmesh(enemyPrefabList[spawnGroupGameObjectIndex.Current], spawnGroupPosition[i]);
                        spawnGroupGameObjectIndex.MoveNext();
                    }
                }
                else
                {
                    for (int i = 0; i < spawnGroupPosition.Count; i++)
                    {
                        SpawnEnemyOntoNavmesh(enemyPrefabList[spawnGroupGameObjectIndex.Current], spawnGroupPosition[i]);
                    }
                    spawnGroupGameObjectIndex.MoveNext();
                }
            }
        }

        private void SpawnEnemyOntoNavmesh(GameObject gameObject, Vector3 position)
        {
            if (!IsServer) return;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 1000f, NavMesh.AllAreas))
            {
                var enemy = NetworkObjectPool.Singleton.GetNetworkObject(gameObject, hit.position, Quaternion.identity);

                var navmeshagent = enemy.GetComponent<NavMeshAgent>();
                navmeshagent.enabled = false;
                navmeshagent.enabled = true;

                EnemyBase enemyBase;
                if (enemy.TryGetComponent<EnemyBase>(out enemyBase))
                {
                    enemyBase.OnEnemyDie += GenerateEnemyDieCallback(enemyBase);
                    spawnedEnemyRef.Add(enemyBase);
                    currentAliveEnemy.Value += 1;
                }

                enemy.Spawn();
            }
        }

        private Action GenerateEnemyDieCallback(EnemyBase enemy)
        {
            void callback()
            {
                OnEnemyDies?.Invoke(enemy);
                spawnedEnemyRef.Remove(enemy);
                currentAliveEnemy.Value -= 1;

                if (currentAliveEnemy.Value == 0)
                {
                    OnAllEnemyDies?.Invoke(); 
                }

                enemy.OnEnemyDie -= callback;
            }
            return callback;
        }

        // I really hope the Garbage Collector can deal with this...
        public IEnumerable<int> LoopingIndexGenerator(int maxExclusive)
        {
            int currentNum = 0;
            while (true)
            {
                yield return currentNum++;
                if (currentNum >= maxExclusive) currentNum = 0;
            }
        }
    }
}
