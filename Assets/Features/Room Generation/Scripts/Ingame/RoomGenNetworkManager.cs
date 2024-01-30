using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RoomGeneration
{
    public class RoomGenNetworkManager : Singleton<RoomGenNetworkManager>
    {
        [HideInInspector]
        public NetworkVariable<int> seed = new NetworkVariable<int>(0);
        [HideInInspector]
        public NetworkVariable<bool> isGeneratedOnServer = new NetworkVariable<bool>(false);
        [ReadOnly]
        [SerializeField]
        private bool isGenerated = false;

        [Header("Settings")]
        public int roomAmount = 10;

        [Header("Requirements")]
        [SerializeField]
        private RoomGenerator roomGenerator;
        [SerializeField]
        private NavigationBaker navigationBaker;

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
            {
                TryGenerate();
            }
        }

        [ContextMenu("Try Generate")]
        public void TryGenerate()
        {
            if (!IsServer)
            {
                Debug.Log("Not server or client, no permissions to generate rooms");
                return;
            }

            GenerateServerRpc();
        }

        [ServerRpc]
        public void GenerateServerRpc()
        {
            // preparing settings and seeds
            seed.Value = Random.Range(int.MinValue, int.MaxValue);
            isGeneratedOnServer.Value = true;

            // tell clients to generate
            GenerateLevelClientRpc(seed.Value);
        }

        [ClientRpc]
        public void GenerateLevelClientRpc(int seedValue)
        {
            float startTime = Time.realtimeSinceStartup;

            // generate rooms
            roomGenerator.StepAddRoom(roomAmount, seedValue);
            navigationBaker.BakeNavMesh();

            Debug.Log("GenerateLevelClientRpc(): Seed = " + seedValue + ", Room Amount = " + roomAmount);
            Debug.Log("Client ID: " + NetworkManager.Singleton.LocalClientId + " generated rooms in " + (Time.realtimeSinceStartup - startTime) + " seconds");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // check if host already generated the room
            if (isGeneratedOnServer.Value)
            {
                GenerateLevelClientRpc(seed.Value);
            }

        }
    }
}