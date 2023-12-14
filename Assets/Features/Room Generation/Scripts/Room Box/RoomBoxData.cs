using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoxData
{
    public HashSet<Vector3Int> roomSpaces;

    public RoomBoxData()
    {
        roomSpaces = new HashSet<Vector3Int>();
    }

    public void AddData(Vector3Int pos)
    {
        roomSpaces.Add(pos);
    }

    public void AddData(RoomBoxData roomBoxData)
    {
        foreach (Vector3Int pos in roomBoxData.roomSpaces)
            roomSpaces.Add(pos);
    }

    public string ToGridString(int gridSize = 12)
    {
        string output = "";
        bool[,] grid = new bool[gridSize, gridSize];

        // set grid array
        foreach (Vector3Int pos in roomSpaces)
        {
            if (pos.y > 0) continue;

            // from [-x, x] to [0, 2x]
            if (pos.x > gridSize / 2 - 1) continue;
            if (pos.z > gridSize / 2 - 1) continue;
            grid[pos.x + gridSize / 2, pos.z + gridSize / 2] = true;
        }

        // add to string
        for (int z = grid.GetLength(1) - 1; z >= 0; z--)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, z])
                {
                    output += ("").PadRight(1);
                    output += ("O").PadRight(0);
                }
                else
                {
                    output += ("").PadRight(1);
                    output += ("x").PadRight(2);
                }
            }
            output += "\n";
        }
        return output;
    }

    public override string ToString()
    {
        string output = "";
        foreach (Vector3Int pos in roomSpaces)
        {
            output += pos.ToString() + "\n";
        }
        return output;
    }
}
