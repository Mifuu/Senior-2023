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
            enemy.OnEnemyDie += SpawnKey;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            enemy.OnEnemyDie -= SpawnKey;
        }

        private void SpawnKey(GameObject killer)
        {
            var keyInstance = Instantiate(keyPrefab, enemy.transform.position, enemy.transform.rotation);
            if (keyInstance.TryGetComponent<NetworkObject>(out NetworkObject obj))
                obj.Spawn();
        }
    }
}
