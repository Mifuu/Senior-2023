using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;

namespace Enemy
{
    public class AssociatorController : NetworkBehaviour
    {
        [Header("Associator Spawn Config")]
        [SerializeField] private VisualEffect associatorPrefab;
        [SerializeField] private EnemyBase enemy;
        [SerializeField] private MinibossFightRangeController rangeController;
        [SerializeField] private PersonalEnemySpawnManager spawnManager;
        [SerializeField] private float yRaise;

        private Dictionary<EnemyBase, VisualEffect> vfxMap = new Dictionary<EnemyBase, VisualEffect>();

        public void Awake()
        {
            if (associatorPrefab == null)
            {
                Debug.LogError("Associator Prefab is null");
                Destroy(this);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SpawnAll(spawnManager.spawnedEnemyRef); // In case the enemy is already spawned
            spawnManager.OnEnemySpawns += SpawnAll;
            spawnManager.OnEnemyDies += DespawnAssociator;
            rangeController.OnMinibossFightEnter += StopAllAssociator;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            foreach (var enemy in spawnManager.spawnedEnemyRef)
                DespawnAssociator(enemy);

            spawnManager.OnEnemySpawns -= SpawnAll;
            spawnManager.OnEnemyDies -= DespawnAssociator;
            rangeController.OnMinibossFightExit -= PlayAllAssociator;
            rangeController.OnMinibossFightEnter -= StopAllAssociator;
        }

        private void PlayAllAssociator()
        {
            Debug.Log("Playing all " + vfxMap.Values.Count + " Ass");
            foreach (VisualEffect vfx in vfxMap.Values)
            {
                if (vfx != null)
                {
                    vfx.Play();
                }
            }

            if (IsServer)
                PlayVfxClientRpc();
        }

        private void StopAllAssociator()
        {
            Debug.Log("Stopping all " + vfxMap.Values.Count + " Ass");
            foreach (VisualEffect vfx in vfxMap.Values)
            {
                if (vfx != null)
                {
                    vfx.Stop();
                }
            }

            if (IsServer)
                StopVfxClientRpc();
        }

        private void DespawnAssociator(EnemyBase enemy)
        {
            if (!IsServer) return;
            if (vfxMap.TryGetValue(enemy, out var vfx))
            {
                vfx.GetComponent<NetworkObject>()?.Despawn(true);
                vfxMap.Remove(enemy);
            }
        }

        private void SpawnAll(List<EnemyBase> enemies)
        {
            if (!IsServer) return;
            for (int i = 0; i < enemies.Count; i++)
                SpawnAssociator(enemies[i]);
        }

        private void SpawnAssociator(EnemyBase leafEnemy)
        {
            if (vfxMap.ContainsKey(leafEnemy)) return;
            var centerLocation = Vector3.Lerp(enemy.transform.position, leafEnemy.transform.position, 0.5f);
            centerLocation.y += yRaise;
            var associatorInstance = Instantiate(associatorPrefab, centerLocation, Quaternion.identity);
            if (associatorInstance.TryGetComponent<NetworkObject>(out var networkObject))
                networkObject.Spawn();
            if (rangeController.isFightActivated.Value) associatorInstance.Stop();
            associatorInstance.transform.LookAt(enemy.transform);
            associatorInstance.transform.eulerAngles = new Vector3(0, associatorInstance.transform.eulerAngles.y, associatorInstance.transform.eulerAngles.z);

            vfxMap.Add(leafEnemy, associatorInstance);
            if (leafEnemy.TryGetComponent<NetworkObject>(out var leaf) && associatorInstance.TryGetComponent<NetworkObject>(out var ass))
                SyncVfxMapClientRpc(leaf, ass);
        }

        [ClientRpc]
        private void SyncVfxMapClientRpc(NetworkObjectReference leafRef, NetworkObjectReference assRef)
        {
            if (leafRef.TryGet(out var networkLeaf) && assRef.TryGet(out var networkAss))
            {
                if (networkLeaf.TryGetComponent<EnemyBase>(out var enemy) && networkAss.TryGetComponent<VisualEffect>(out var associator))
                {
                    vfxMap.Add(enemy, associator);
                }
            }
        }

        [ClientRpc]
        private void DeleteVfxMapClientRpc(NetworkObjectReference enemyRef)
        {
            if (enemyRef.TryGet(out var network))
            {
                if (network.TryGetComponent<EnemyBase>(out var enemy) && vfxMap.TryGetValue(enemy, out var vfx))
                {
                    vfx.GetComponent<NetworkObject>()?.Despawn(true);
                    vfxMap.Remove(enemy);
                }
            }
        }

        [ClientRpc]
        private void PlayVfxClientRpc()
        {
            PlayAllAssociator();
        }

        [ClientRpc]
        private void StopVfxClientRpc()
        {
            StopAllAssociator();
        }
    }
}
