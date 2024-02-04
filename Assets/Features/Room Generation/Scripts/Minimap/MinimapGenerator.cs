using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration.Minimap
{
    public class MinimapGenerator
    {
        public static int[,] GetIndexGrid(RoomGenerator roomGenerator, int gridSize)
        {
            // get array. key is coord, value is room index
            var roomGrid = new Dictionary<Vector3Int, int>(roomGenerator.roomGrid);
            var roomCoords = new Vector3Int[roomGrid.Count];
            var roomIndices = new int[roomGrid.Count];
            int i = 0;
            foreach (var kvp in roomGrid)
            {
                roomCoords[i] = kvp.Key;
                roomIndices[i] = kvp.Value;
                i++;
            }

            // get max and min
            int maxAbsX = int.MinValue;
            int maxAbsY = int.MinValue;
            foreach (var coord in roomCoords)
            {
                if (Mathf.Abs(coord.x) > maxAbsX) maxAbsX = Mathf.Abs(coord.x);
                if (Mathf.Abs(coord.y) > maxAbsY) maxAbsY = Mathf.Abs(coord.y);
            }

            if (maxAbsX + 1 > gridSize / 2 || maxAbsY + 1 > gridSize / 2)
            {
                Debug.Log("MinimapDisplay: Room grid size is too big for the setting grid size. overriding...");
                gridSize = Mathf.Max(maxAbsX, maxAbsY) * 2 + 2;
            }
            // create roomGridRaw
            int[,] output = new int[gridSize, gridSize];
            int lenY = output.GetLength(0);
            int lenX = output.GetLength(1);
            for (int y = 0; y < lenY; y++)
            {
                for (int x = 0; x < lenX; x++)
                {
                    output[y, x] = -1;
                }
            }

            for (int y = 0; y < lenY; y++)
            {
                for (int x = 0; x < lenX; x++)
                {
                    Vector3Int key = GetMapCoord(x, y, lenX, lenY);
                    if (roomGrid.ContainsKey(key))
                        output[y, x] = roomGrid[key];
                }
            }

            return output;
        }

        public static bool[,] GetBoolGridFromIndexGrid(int[,] indexGrid)
        {
            bool[,] output = new bool[indexGrid.GetLength(0), indexGrid.GetLength(1)];
            for (int y = 0; y < output.GetLength(0); y++)
            {
                for (int x = 0; x < output.GetLength(1); x++)
                {
                    output[y, x] = indexGrid[y, x] >= 0;
                }
            }
            return output;
        }

        public static Texture2D CreateTextureFromBoolGrid(bool[,] boolGrid)
        {
            Texture2D output = new Texture2D(boolGrid.GetLength(0), boolGrid.GetLength(1));
            for (int y = 0; y < output.height; y++)
            {
                for (int x = 0; x < output.width; x++)
                {
                    output.SetPixel(x, y, boolGrid[y, x] ? Color.white : Color.black);
                }
            }
            output.filterMode = FilterMode.Point;
            output.Apply();
            return output;
        }

        public static Texture2D CreateRoomOutlineFromIndexGrid(int[,] indexGrid, int unitSize = 7, int outlineSize = 1, int doorRadius = 2, RoomGenerator roomGenerator = null)
        {
            Texture2D output = new Texture2D(indexGrid.GetLength(0) * unitSize, indexGrid.GetLength(1) * unitSize);
            for (int y = 0; y < indexGrid.GetLength(0); y++)
            {
                for (int x = 0; x < indexGrid.GetLength(1); x++)
                {   // foreach area
                    // corners positions
                    Vector2Int bottomLeft = new Vector2Int(x * unitSize, y * unitSize);
                    Vector2Int topLeft = new Vector2Int(x * unitSize, (y + 1) * unitSize - 1);
                    Vector2Int topRight = new Vector2Int((x + 1) * unitSize - 1, (y + 1) * unitSize - 1);
                    Vector2Int bottomRight = new Vector2Int((x + 1) * unitSize - 1, y * unitSize);

                    // fill
                    FillInPixel(ref output, bottomLeft, topRight, Color.clear);

                    // draw grid
                    FillInPixel(ref output, bottomLeft, topLeft, Color.gray);
                    FillInPixel(ref output, bottomLeft, bottomRight, Color.gray);
                    // fill
                    FillInPixel(ref output, bottomLeft, bottomLeft, Color.black);

                    // continue if empty
                    int index = indexGrid[y, x];
                    if (index < 0) continue;

                    // fill
                    FillInPixel(ref output, bottomLeft, topRight, Color.gray);

                    // fill in walls if the connected area is of different index
                    // left
                    if (indexGrid[y, x - 1] != index)
                        FillInPixel(ref output, bottomLeft, topLeft + new Vector2Int(outlineSize - 1, 0), Color.white);
                    // top
                    if (indexGrid[y + 1, x] != index)
                        FillInPixel(ref output, topLeft, topRight - new Vector2Int(0, outlineSize - 1), Color.white);
                    // right
                    if (indexGrid[y, x + 1] != index)
                        FillInPixel(ref output, topRight, bottomRight - new Vector2Int(outlineSize - 1, 0), Color.white);
                    // bottom
                    if (indexGrid[y - 1, x] != index)
                        FillInPixel(ref output, bottomRight, bottomLeft + new Vector2Int(0, outlineSize - 1), Color.white);

                    // fill
                    FillInPixel(ref output, bottomLeft, bottomLeft, Color.black);
                }
            }

            // carve out doors
            Vector3 offset = new Vector3(0, 0.5f, 0);
            for (int y = 0; y < indexGrid.GetLength(0); y++)
            {
                for (int x = 0; x < indexGrid.GetLength(1); x++)
                {
                    // carve out doors if is a door at area
                    Vector3Int coord = GetMapCoord(x, y, indexGrid.GetLength(1), indexGrid.GetLength(0));
                    if (!roomGenerator.IsConnectedDoor(coord + offset)) continue;
                    Vector2Int a = new Vector2Int(x * unitSize, y * unitSize) + Vector2Int.one * (doorRadius - 1);
                    Vector2Int b = new Vector2Int(x * unitSize, y * unitSize) - Vector2Int.one * doorRadius;
                    FillInPixel(ref output, a, b, Color.gray);
                }
            }
            output.filterMode = FilterMode.Point;
            output.Apply();
            return output;
        }

        private static Vector3Int GetMapCoord(int x, int y, int[,] indexGrid)
        {
            return new Vector3Int(x - indexGrid.GetLength(1) / 2, 0, y - indexGrid.GetLength(0) / 2);
        }

        private static Vector3Int GetMapCoord(int x, int y, int lenX, int lenY)
        {
            return new Vector3Int(x - lenX / 2, 0, y - lenY / 2);
        }

        public static void FillInPixel(ref Texture2D texture, Vector2Int pos1, Vector2Int pos2, Color color)
        {
            FillInPixel(ref texture, pos1.x, pos1.y, pos2.x, pos2.y, color);
        }

        public static void FillInPixel(ref Texture2D texture, int pos1X, int pos1Y, int pos2X, int pos2Y, Color color)
        {
            int minX = Mathf.Min(pos1X, pos2X);
            int maxX = Mathf.Max(pos1X, pos2X);
            int minY = Mathf.Min(pos1Y, pos2Y);
            int maxY = Mathf.Max(pos1Y, pos2Y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}
