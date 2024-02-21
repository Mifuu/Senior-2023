using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{
    public class RoomSpawnerCollider : MonoBehaviour
    {
        public RoomSpawner roomSpawner;

        public void Init(RoomSpawner roomSpawner)
        {
            this.roomSpawner = roomSpawner;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out PlayerManager player))
                roomSpawner.OnPlayerEnterRoom();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out PlayerManager player))
                roomSpawner.OnPlayerExitRoom();
        }
    }
}