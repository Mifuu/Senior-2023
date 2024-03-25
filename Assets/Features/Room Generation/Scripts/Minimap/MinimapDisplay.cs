using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomGeneration.Minimap
{
    public class MinimapDisplay : MonoBehaviour
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
        }

        public void Test()
        {
            Debug.Log("test4");
        }

        public Vector2Int GetIndexGridSize()
        {
            return new Vector2Int(indexGrid.GetLength(1), indexGrid.GetLength(0));
        }

        public int GetGridSize()
        {
            return gridSize;
        }
    }
}
