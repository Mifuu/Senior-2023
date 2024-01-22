using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class RoomGenSpawnEnemy : NetworkBehaviour
    {
        public RoomGenerator roomGenerator;
        public EnemySpawnManager enemySpawnManager;

        [Header("Spawn Settings")]
        public GameObject[] enemyPrefabs;
        public int monsterAmount = 5;
        public Vector2 spawnRadiusRange = new Vector2(5, 10);

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
            {
                ImidiateSpawnEnemy();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        [ContextMenu("Spawn Enemy")]
        public void ImidiateSpawnEnemy()
        {
            // check requirements
            /*
            if (!IsServer)
            {
                Debug.Log("Not Server");
                return;
            }
            */

            if (roomGenerator == null)
            {
                Debug.Log("RoomGenerator Not Found");
                return;
            }

            if (!roomGenerator.HasGenerated)
            {
                Debug.Log("RoomGenerator has not generate room");
                return;
            }

            PlayerManager[] playerManagers = FindObjectsOfType<PlayerManager>();

            foreach (var p in playerManagers)
            {
                // get player's position
                Vector3 playerPos = p.transform.position;

                // get current room position
                Vector3Int currentRoomPosition = roomGenerator.GetRoomCoordFromPosition(playerPos);

                for (int i = 0; i < monsterAmount; i++)
                {
                    // get random room position that's within radius
                    Vector3Int randomRoomPosition = roomGenerator.GetRandomRoomWithinRadius(currentRoomPosition, spawnRadiusRange);
                    if (randomRoomPosition == currentRoomPosition)
                    {
                        Debug.Log("No viable spawn position found");
                        return;
                    }
                    Vector3 randomRoomPositionWorld = roomGenerator.GetPositionFromRoomCoord(randomRoomPosition);

                    // spawn random monster
                    int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                    GameObject enemy = enemyPrefabs[randomEnemyIndex];
                    enemySpawnManager.SpawnEnemy(enemy, randomRoomPositionWorld);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadiusRange.x);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadiusRange.y);
        }
    }
}