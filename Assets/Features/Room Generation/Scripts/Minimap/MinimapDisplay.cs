using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public const int DEFAULT_GRID_SIZE = 100;

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
            Debug.Log($"[MinimapDisplay.Generate] gridSize: {gridSize}, textureBytes: {m.TextureBytes.Length}, compressedTextureBytes: {m.CompressedTextureBytes.Length}");
            if (IsServer) SetMinimapClientRpc(m);
        }

        [ClientRpc]
        public void SetMinimapClientRpc(MinimapInfoNetwork minimapInfoNetwork)
        {
            Debug.Log($"[MinimapDisplay.SetMinimapClientRpc] gridSize: {minimapInfoNetwork.GridSize}, textureBytes: {minimapInfoNetwork.TextureBytes.Length}, compressedTextureBytes: {minimapInfoNetwork.CompressedTextureBytes.Length}");
            texture = minimapInfoNetwork.GetTexture2D();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;

            gridSize = minimapInfoNetwork.GridSize;
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
            private int gridSize;
            public int GridSize => gridSize;
            private byte[] compressedTextureBytes;
            public byte[] CompressedTextureBytes => compressedTextureBytes;
            private byte[] textureBytes;
            public byte[] TextureBytes
            {
                get
                {
                    if (textureBytes == null || textureBytes.Length == 0)
                    {
                        textureBytes = DecompressBytes(compressedTextureBytes);
                    }
                    return textureBytes;
                }
            }

            public MinimapInfoNetwork(int gridSize, Texture2D texture)
            {
                this.gridSize = gridSize;
                this.textureBytes = texture.EncodeToPNG();
                this.compressedTextureBytes = CompressBytes(textureBytes);
            }

            public Texture2D GetTexture2D()
            {
                Texture2D texture = new Texture2D(gridSize, gridSize);
                texture.LoadImage(TextureBytes);
                return texture;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref gridSize);
                // serializer.SerializeValue(ref textureBytes);
                serializer.SerializeValue(ref compressedTextureBytes);
            }
        }

        private static byte[] CompressBytes(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray();
            }
        }

        private static byte[] DecompressBytes(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(decompressedStream);
                        return decompressedStream.ToArray();
                    }
                }
            }
        }
    }
}
