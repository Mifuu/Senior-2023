using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomGeneration.Minimap
{
    public class MinimapDisplay : MonoBehaviour
    {
        [Header("Default Values")]
        public const int DEFAULT_GRID_SIZE = 64;

        [Header("Requirements")]
        public RoomGenerator roomGenerator;
        public MinimapEntityDisplay minimapEntityDisplay;
        public Image image;

        // [Header("Cache")]
        private Texture2D texture;
        private int gridSize;

        // cache data
        Dictionary<Vector3Int, int> roomGrid = new Dictionary<Vector3Int, int>();
        int[,] indexGrid;
        bool[,] boolGrid;

        public void Generate()
        {
            indexGrid = MinimapGenerator.GetIndexGrid(roomGenerator, DEFAULT_GRID_SIZE);
            boolGrid = MinimapGenerator.GetBoolGridFromIndexGrid(indexGrid);

            // Texture2D texture = MinimapGenerator.CreateTextureFromBoolGrid(boolGrid);
            texture = MinimapGenerator.CreateRoomOutlineFromIndexGrid(indexGrid, 13, 1, 5, roomGenerator);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;

            gridSize = indexGrid.GetLength(1);
            Debug.Log("texture.width: " + texture.width + ", indexGrid.GetLength(1): " + indexGrid.GetLength(1) + ", gridSize: " + gridSize);

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
    }
}
