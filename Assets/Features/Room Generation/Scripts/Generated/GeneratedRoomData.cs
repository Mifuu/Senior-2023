using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneratedRoomData
{
    public RoomData roomData;
    public int rot90Factor = 0;
    public Vector3Int placementOffset;

    public GameObject gameObject;
    public List<Vector3Int> roomSpaces;
    public List<GeneratedDoorData> generatedDoorDatas;

    public GeneratedRoomData(RoomData roomData, int rot90Factor, GameObject gameObject, Vector3Int placementOffset)
    {
        this.roomData = roomData;
        this.rot90Factor = rot90Factor;
        this.placementOffset = placementOffset;

        this.gameObject = gameObject;
        roomSpaces = new List<Vector3Int>();
        AddRoomSpaces();
        generatedDoorDatas = new List<GeneratedDoorData>();
        AddRoomDoorDatas();
    }

    public void AddRoomSpaces()
    {
        roomSpaces.Clear();
        foreach (Vector3Int pos in roomData.roomBoxData.roomSpaces)
        {
            Vector3Int rotatedPos = GetRot90Pos(pos, rot90Factor);
            roomSpaces.Add(rotatedPos + placementOffset);
        }
    }

    public void AddRoomDoorDatas()
    {
        generatedDoorDatas.Clear();
        foreach (DoorData doorData in roomData.roomDoorData.doorDatas)
        {
            Vector3 rotatedPos = GetRot90Pos(doorData.coord, rot90Factor);
            Vector3 worldCoord = rotatedPos + placementOffset;

            GeneratedDoorData generatedDoorData = new GeneratedDoorData();
            generatedDoorData.generatedRoomData = this;
            generatedDoorData.localCoord = rotatedPos;
            generatedDoorData.worldCoord = worldCoord;
            generatedDoorData.doorDir = (DoorDir)Mod((int)doorData.doorDir + rot90Factor, 4);
            generatedDoorDatas.Add(generatedDoorData);
        }
    }

    public Vector3 GetRot90Pos(Vector3 pos, int rot90)
    {
        switch (rot90)
        {
            case 0:
                return pos;
            case 1:
                return new Vector3(pos.z, pos.y, -pos.x);
            case 2:
                return new Vector3(-pos.x, pos.y, -pos.z);
            case 3:
                return new Vector3(-pos.z, pos.y, pos.x);
        }

        return pos;
    }

    public Vector3Int GetRot90Pos(Vector3Int pos, int rot90)
    {
        switch (rot90)
        {
            case 0:
                return pos;
            case 1:
                return new Vector3Int(pos.z, pos.y, -pos.x);
            case 2:
                return new Vector3Int(-pos.x, pos.y, -pos.z);
            case 3:
                return new Vector3Int(-pos.z, pos.y, pos.x);
        }

        return pos;
    }

    int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}

[System.Serializable]
public class GeneratedDoorData
{
    public GeneratedRoomData generatedRoomData;
    public Vector3 localCoord;
    public Vector3 worldCoord;
    public DoorDir doorDir;
}

[System.Serializable]
public class CandidateDoor
{
    public DoorData doorData;
    public int rot90Factor = 0;

    public CandidateDoor(DoorData doorData, int rot90Factor)
    {
        this.doorData = doorData;
        this.rot90Factor = rot90Factor;
    }
}