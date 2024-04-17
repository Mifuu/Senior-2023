using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class MinibossKeyDropper : NetworkBehaviour
    {
        [SerializeField] private GameObject keyPrefab;
        [SerializeField] private EnemyBase enemy;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            enemy.OnEnemyDie += SpawnKeyServerRpc;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            enemy.OnEnemyDie -= SpawnKeyServerRpc;
        }

        [ServerRpc]
        private void SpawnKeyServerRpc()
        {
            Debug.Log("Spawning Key");
            var keyInstance = Instantiate(keyPrefab, enemy.transform.position, enemy.transform.rotation);
            if (keyInstance.TryGetComponent(out NetworkObject obj))
            {
                obj.Spawn();
            }
            else
            {
                Debug.LogWarning("The spawned key prefab is missing a NetworkObject component.");
                Destroy(keyInstance); 
            }
        }
    }
}
