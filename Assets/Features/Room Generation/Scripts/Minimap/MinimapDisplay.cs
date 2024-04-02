using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace RoomGeneration.Minimap
{
    public class MinimapDisplay : NetworkBehaviour
    {
        public static MinimapDisplay instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        [Header("Default Values")]
        public const int DEFAULT_GRID_SIZE = 40;

        [Header("Requirements")]
        public RoomGenerator roomGenerator;
        public MinimapEntityDisplay minimapEntityDisplay;
        public Image image;

        [Header("Minimap Settings")]
        public MinimapSettings settings;

        // [Header("Cache")]
        private Texture2D texture;
        private int gridSize;

        // cache data
        Dictionary<Vector3Int, int> roomGrid = new Dictionary<Vector3Int, int>();
        int[,] indexGrid;

        void Start()
        {
            // RoomGenNetworkManager.Instance.onGenerateLevel += Generate;
        }

        public void Generate()
        {
            texture = MinimapGenerator.GenerateTexture(roomGenerator, settings);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;

            // gridSize = indexGrid.GetLength(1);
            gridSize = texture.width / settings.unitSize;
            // Debug.Log("texture.width: " + texture.width + ", indexGrid.GetLength(1): " + indexGrid.GetLength(1) + ", gridSize: " + gridSize);

            minimapEntityDisplay.Init();

            MinimapInfoNetwork m = new MinimapInfoNetwork(gridSize, texture);
            Debug.Log("tesssst send " + m.textureBytes.Length);
            if (IsServer) SetMinimapClientRpc(m);
        }

        [ClientRpc]
        public void SetMinimapClientRpc(MinimapInfoNetwork minimapInfoNetwork)
        {
            Debug.Log("tesssst " + minimapInfoNetwork.textureBytes.Length);
            texture = minimapInfoNetwork.GetTexture2D();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;

            gridSize = minimapInfoNetwork.gridSize;
            // Debug.Log("texture.width: " + texture.width + ", indexGrid.GetLength(1): " + indexGrid.GetLength(1) + ", gridSize: " + gridSize);

            minimapEntityDisplay.Init();
        }

        public Vector2Int GetIndexGridSize()
        {
            return new Vector2Int(indexGrid.GetLength(1), indexGrid.GetLength(0));
        }

        public int GetGridSize()
        {
            return gridSize;
        }

        public struct MinimapInfoNetwork : INetworkSerializable
        {
            public int gridSize;
            public byte[] textureBytes;

            public MinimapInfoNetwork(int gridSize, Texture2D texture)
            {
                this.gridSize = gridSize;
                this.textureBytes = texture.EncodeToPNG();
            }

            public Texture2D GetTexture2D()
            {
                Texture2D texture = new Texture2D(gridSize, gridSize);
                texture.LoadImage(textureBytes);
                return texture;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref gridSize);
                serializer.SerializeValue(ref textureBytes);
            }
        }
    }
}
