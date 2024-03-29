using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using Enemy;

namespace RoomGeneration
{
    public class RoomSpawner : MonoBehaviour
    {
        // Debug
        public static List<Vector3> checkPoses = new List<Vector3>();

        [Header("Requirements")]
        public RoomDataPlotter roomDataPlotter;
        RoomGenerator roomGenerator;

        [Header("Settings")]
        public LayerMask raycastMask;
        public float distanceFromRaycast = 2;
        public Vector2Int spawnCountRange = new Vector2Int(10, 10);
        public EnemyRandomPool enemyPrefabPool;

        // playtime cache
        List<RoomSpawnerCollider> roomSpawnerColliders = new List<RoomSpawnerCollider>();

        public bool HasPlayer { get => playerEnterCount > playerExitCount; }
        [ReadOnly]
        public float timeSincePlayerExit = -1f;
        [ReadOnly]
        public float timeSinceSpawn = -1f;

        private int playerEnterCount;
        private int playerExitCount;

        void Start()
        {
            roomGenerator = RoomGenerator.Instance;

            // add RoomSpawnerCollider components to all room boxes of RoomDataPlotter
            foreach (GameObject box in roomDataPlotter.roomBoxes)
            {
                RoomSpawnerCollider roomSpawnerCollider = box.AddComponent<RoomSpawnerCollider>();
                roomSpawnerCollider.Init(this);
                roomSpawnerColliders.Add(roomSpawnerCollider);
            }
        }

        private void Update()
        {
            if (timeSincePlayerExit >= 0)
            {
                timeSinceSpawn += Time.deltaTime;
            }

            if (timeSinceSpawn >= 0)
            {
                timeSinceSpawn += Time.deltaTime;
            }
        }

        // called by RoomSpawnerColliders
        public void OnPlayerEnterRoom()
        {
            playerEnterCount++;
            timeSincePlayerExit = -1;

            // check if haven't spawn yet
            if (timeSinceSpawn <= 0)
            {
                // spawn enemies
                // Debug.Log("Spawning enemies...");
                StartCoroutine(SpawnEnemiesCoroutine());
            }
        }

        // called by RoomSpawnerColliders
        public void OnPlayerExitRoom()
        {
            playerExitCount--;
            if (!HasPlayer)
                timeSincePlayerExit = 0.1f;
        }

        IEnumerator SpawnEnemiesCoroutine()
        {
            yield return new WaitForSeconds(0.4f);
            if (HasPlayer)
            {
                SpawnEnemies();
                timeSinceSpawn = 0.1f;
            }
        }

        public void SpawnEnemies()
        {
            // shoot raycast in the lower half of the room
            Vector3 raycastOrigin = transform.position;
            int count = Random.Range(spawnCountRange.x, spawnCountRange.y);

            RaycastHit hit = new RaycastHit();
            for (int i = 0; i < count; i++)
            {
                Vector3 raycastDirection = RandomPointOnUnitHemisphere();
                Physics.Raycast(raycastOrigin, raycastDirection, out hit, 100f, raycastMask);

                if (hit.collider != null)
                {
                    // distance from raycast hit
                    // Vector3 spawnPosition = hit.point;
                    Vector3 spawnPosition = hit.point + hit.normal * distanceFromRaycast;
                    checkPoses.Add(spawnPosition);
                    // Vector3 spawnPosition = transform.position;

                    // spawn enemy
                    GameObject enemyPrefab = enemyPrefabPool.GetRandomPrefab();
                    NavMeshHit navHit;
                    if (enemyPrefab != null && NavMesh.SamplePosition(spawnPosition, out navHit, 1000f, NavMesh.AllAreas))
                    {
                        var enemy = NetworkObjectPool.Singleton.GetNetworkObject(enemyPrefab, navHit.position, Quaternion.identity);

                        var navmeshagent = enemy.GetComponent<NavMeshAgent>();
                        navmeshagent.enabled = false;
                        navmeshagent.enabled = true;

                        enemy.Spawn();
                        enemy.transform.position = spawnPosition;

                        // GameObject e = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                        // e.GetComponent<NetworkObject>().Spawn();

                        // EnemySpawnManager.Singleton.SpawnEnemy(enemyPrefab, spawnPosition);
                    }
                    else
                    {
                        Debug.LogWarning("No enemy prefab found in the pool");
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        // Function to generate a random point on the unit hemisphere
        Vector3 RandomPointOnUnitHemisphere()
        {
            // Generate random angles
            float theta = Random.Range(0f, 2f * Mathf.PI);  // Longitude angle
            float phi = Random.Range(0f, 0.5f * Mathf.PI);  // Latitude angle (restricted to upper hemisphere)

            // Convert spherical coordinates to Cartesian coordinates
            float x = Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = -Mathf.Cos(phi);
            float z = Mathf.Sin(phi) * Mathf.Sin(theta);

            return new Vector3(x, y, z);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Vector3 pos in checkPoses)
            {
                Gizmos.DrawSphere(pos, 0.3f);
            }
        }
    }
}