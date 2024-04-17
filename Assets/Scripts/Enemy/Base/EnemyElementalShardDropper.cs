using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyElementalShardDropper : NetworkBehaviour
    {
        [SerializeField] private ElementAttachable element;
        [SerializeField] private EnemyBase enemy;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            enemy.OnEnemyDie += SpawnShardsServerRpc;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!IsServer) return;
            enemy.OnEnemyDie -= SpawnShardsServerRpc;
        }

        [ServerRpc]
        private void SpawnShardsServerRpc()
        {
            var shardGameObject = ElementalShardManager.Singleton.GetShardOfElement(element.element);
            if (shardGameObject == null)
                return;
            var insGameObject = Instantiate(shardGameObject, enemy.transform.position, enemy.transform.rotation);
            if (insGameObject.TryGetComponent(out NetworkObject obj))
            {
                obj.Spawn();
            }
            else
            {
                Debug.LogWarning("The spawned key prefab is missing a NetworkObject component.");
                Destroy(insGameObject);
            }
        }
    }
}
