using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using RoomGeneration.Minimap;

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
        public int playSpawnRoomAmount = 4;

        [Header("Requirements")]
        [SerializeField]
        public RoomGenerator roomGenerator;
        [SerializeField]
        private NavigationBaker navigationBaker;


        public delegate void OnGenerateLevel();
        public OnGenerateLevel onGenerateLevel;

        private void Awake()
        {
            if (!IsServer)
            {
                // disable the room generator and navigation baker if not server
                roomGenerator.enabled = false;
                navigationBaker.enabled = false;
            }
        }

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
                Debug.Log("Not server, no permissions to generate rooms");
                return;
            }

            // GenerateServerRpc();
            Generate();
        }

        public void Generate()
        {
            // preparing settings and seeds
            seed.Value = Random.Range(int.MinValue, int.MaxValue);
            isGeneratedOnServer.Value = true;

            // generate rooms
            float startTime = Time.realtimeSinceStartup;

            // roomGenerator.StepAddRoom(roomAmount, seed.Value);
            roomGenerator.GenerateLevel();
            navigationBaker.BakeNavMesh();

            Debug.Log("RoomGenNetworkManager.GenerateServerRPC(): Seed = " + seed.Value + ", Room Amount = " + roomAmount);
            Debug.Log("Client ID: " + NetworkManager.Singleton.LocalClientId + " generated rooms in " + (Time.realtimeSinceStartup - startTime) + " seconds");

            onGenerateLevel?.Invoke();
            MinimapDisplay.instance.Generate();
        }

        /*
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
        */
    }
}