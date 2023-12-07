using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TerrainNetworkManager : Singleton<TerrainNetworkManager>
{
    // public NetworkVariable<TerrainInfo> terrainInfo = new NetworkVariable<TerrainInfo>(new TerrainInfo());
    public NetworkVariable<int> seed = new NetworkVariable<int>(0);

    public void TryGenerate()
    {
        Debug.Log("TryGenerate()");

        if (!IsServer)
        {
            Debug.Log("Not server or client, no permissions to generate terrain");
            return;
        }

        GenerateTerrainServerRpc();
    }

    [ServerRpc]
    void GenerateTerrainServerRpc()
    {
        Debug.Log("TerrainNetworkManager.GenerateTerrainServerRpc()");

        /*
        // set up terrain information
        TerrainInfo _terrainInfo = new TerrainInfo()
        {
            seed = Random.Range(int.MinValue, int.MaxValue),
            serializedHeightCurve = new SerializedAnimationCurve()
            {
                keyFrameTimes = new float[] { 0, 1 },
                keyFrameValues = new float[] { 0, 2 }
            }
        };

        Debug.Log("[Server] seed = " + _terrainInfo.seed);

        // set network variable
        terrainInfo.Value = _terrainInfo;
        */

        seed.Value = Random.Range(int.MinValue, int.MaxValue);
        GenerateTerrainClientRpc(seed.Value);
    }

    [ClientRpc]
    void GenerateTerrainClientRpc(int seedValue)
    {
        TerrainManager.Instance.SetSeed(seedValue);
        TerrainManager.Instance.Generate();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (seed.Value != 0) // if seed is not 0, then it is already generated
        {
            TerrainManager.Instance.SetSeed(seed.Value);
            TerrainManager.Instance.Generate();
        }
        // terrainInfo.OnValueChanged += OnTerrainInfoChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // terrainInfo.OnValueChanged -= OnTerrainInfoChanged;
    }

    /*
    public void OnTerrainInfoChanged(TerrainInfo previousValue, TerrainInfo newValue)
    {
        Debug.Log("TerrainNetworkManager.OnTerrainInfoChanged()");

        // generate terrain
        Debug.Log("OnTerrainInfoChanged: " + newValue.seed + ", " + AnimationCurveSerializer.Deserialize(newValue.serializedHeightCurve).Evaluate(1f));
    }

    [System.Serializable]
    public struct TerrainInfo : INetworkSerializable
    {
        public int seed;
        public SerializedAnimationCurve serializedHeightCurve;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref seed);

            serializedHeightCurve.NetworkSerialize(serializer);
        }
    }
    */
}
