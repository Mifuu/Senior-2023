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
        public Image image;

        // cache data
        Dictionary<Vector3Int, int> roomGrid = new Dictionary<Vector3Int, int>();
        int[,] indexGrid;
        bool[,] boolGrid;

        public void Generate()
        {
            indexGrid = MinimapGenerator.GetIndexGrid(roomGenerator, DEFAULT_GRID_SIZE);
            boolGrid = MinimapGenerator.GetBoolGridFromIndexGrid(indexGrid);

            // Texture2D texture = MinimapGenerator.CreateTextureFromBoolGrid(boolGrid);
            Texture2D texture = MinimapGenerator.CreateRoomOutlineFromIndexGrid(indexGrid, 13, 1, 5, roomGenerator);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }
}
