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
            enemy.OnEnemyDie += SpawnShards;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!IsServer) return;
            enemy.OnEnemyDie -= SpawnShards;
        }

        private void SpawnShards(GameObject killer)
        {
            if (killer == null) return;
            var shardGameObject = ElementalShardManager.Singleton.GetShardOfElement(element.element);
            if (shardGameObject == null)
                return;
            var insGameObject = Instantiate(shardGameObject, enemy.transform.position, enemy.transform.rotation);
            if (insGameObject.TryGetComponent<NetworkObject>(out NetworkObject obj))
            {
                obj.Spawn();
            }
        }
    }
}
