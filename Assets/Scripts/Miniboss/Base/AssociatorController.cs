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
                Destroy(this);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SpawnAll(spawnManager.spawnedEnemyRef); // In case the enemy is already spawned
            spawnManager.OnEnemySpawns += SpawnAll;
            spawnManager.OnEnemyDies += DespawnAssociator;
            rangeController.OnMinibossFightExit += PlayAllAssociator;
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
            foreach (VisualEffect vfx in vfxMap.Values)
                vfx.Play();
        }

        private void StopAllAssociator()
        {
            foreach (VisualEffect vfx in vfxMap.Values)
                vfx.Stop();
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
            associatorInstance.transform.LookAt(enemy.transform);
            associatorInstance.transform.eulerAngles = new Vector3(0, associatorInstance.transform.eulerAngles.y, associatorInstance.transform.eulerAngles.z);

            vfxMap.Add(leafEnemy, associatorInstance);
        }
    }
}
