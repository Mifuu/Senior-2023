using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Room Generation/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomBoxData roomBoxData;
    public RoomDoorData roomDoorData;

    public GameObject roomPrefab;

    public List<DoorData> GetDoorDatas(DoorData.DoorDir doorDir)
    {
        List<DoorData> doorDatas = new List<DoorData>();
        foreach (var d in roomDoorData.doorDatas)
        {
            if (d.doorDir == doorDir)
                doorDatas.Add(d);
        }
        return doorDatas;
    }
}

[System.Serializable]
public class RoomBoxData
{
    public List<Vector3Int> roomSpaces;

    public RoomBoxData()
    {
        roomSpaces = new List<Vector3Int>();
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

    public void Clear()
    {
        roomSpaces.Clear();
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

[System.Serializable]
public class RoomDoorData
{
    public RoomData parentRoom;
    public List<DoorData> doorDatas;

    public RoomDoorData(RoomData parentRoom)
    {
        doorDatas = new List<DoorData>();
        this.parentRoom = parentRoom;
    }

    public void AddData(Vector3 pos, DoorData.DoorDir doorDir)
    {
        doorDatas.Add(new DoorData(pos, doorDir, parentRoom));
    }

    public void AddData(RoomDoorData roomDoorData)
    {
        foreach (var d in roomDoorData.doorDatas)
            doorDatas.Add(d);
    }

    public void Clear()
    {
        doorDatas.Clear();
    }

    public override string ToString()
    {
        string output = "";
        output += "Door Count: " + doorDatas.Count + "\n";
        foreach (var d in doorDatas)
        {
            output += d.coord.ToString() + "\n";
        }
        return output;
    }
}

[System.Serializable]
public class DoorData
{
    public RoomData parentRoom;
    public Vector3 coord;
    public enum DoorDir { ZPos, XPos, ZNeg, XNeg };
    public DoorDir doorDir;

    public DoorData(Vector3 coord, DoorDir doorDir, RoomData parentRoom)
    {
        this.coord = coord;
        this.doorDir = doorDir;
        this.parentRoom = parentRoom;
    }

    public DoorData(DoorData data)
    {
        coord = data.coord;
        doorDir = data.doorDir;
        parentRoom = data.parentRoom;
    }
}