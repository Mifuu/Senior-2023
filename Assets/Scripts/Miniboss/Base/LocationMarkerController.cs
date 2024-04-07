using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class LocationMarkerController : NetworkBehaviour
    {
        [Header("Location Marker Setup")]
        [SerializeField] private Animator locationMarkerPrefab;
        [SerializeField] private MinibossFightRangeController rangeController;
        private Animator locationMarkerInstance;
        public readonly int spawnAnimation = Animator.StringToHash("Spawn");
        public readonly int despawnAnimation = Animator.StringToHash("Despawn");

        public override void OnNetworkSpawn()
        {
            base.OnNetworkDespawn();
            SpawnMarker();
            rangeController.OnMinibossFightExit += SpawnMarker;
            rangeController.OnMinibossFightEnter += DespawnMarker;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            rangeController.OnMinibossFightExit -= SpawnMarker;
            rangeController.OnMinibossFightEnter -= DespawnMarker;
        }

        private void SpawnMarker()
        {
            if (!IsServer) return;
            var markerInstance = Instantiate(locationMarkerPrefab, transform.position, locationMarkerPrefab.transform.rotation);
            if (markerInstance.TryGetComponent<NetworkObject>(out var networkObject))
                networkObject.Spawn();
            markerInstance.SetTrigger(spawnAnimation);
            locationMarkerInstance = markerInstance;
        }

        private void DespawnMarker()
        {
            if (!IsServer) return;
            if (locationMarkerInstance == null) return;
            locationMarkerInstance.SetTrigger(despawnAnimation);
        }
    }
}
