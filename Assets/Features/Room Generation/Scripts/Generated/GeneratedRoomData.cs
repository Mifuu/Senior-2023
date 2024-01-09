using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGenUtil;

[System.Serializable]
public class GeneratedRoomData
{
    public int id;

    public RoomData roomData;
    public int rot90Factor = 0;
    public Vector3Int placementOffset;

    public GameObject gameObject;
    public List<Vector3Int> roomSpaces;
    public List<GeneratedDoorData> generatedDoorDatas;

    public GeneratedRoomData(RoomData roomData, int id, int rot90Factor, GameObject gameObject, Vector3Int placementOffset)
    {
        this.id = id;

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
            Vector3Int rotatedPos = RoomRotUtil.GetRot90Grid(pos, rot90Factor);
            roomSpaces.Add(rotatedPos + placementOffset);
        }
    }

    public void AddRoomDoorDatas()
    {
        generatedDoorDatas.Clear();
        foreach (DoorData doorData in roomData.roomDoorData.doorDatas)
        {
            Vector3 rotatedPos = RoomRotUtil.GetRot90Pos(doorData.coord, rot90Factor);
            Vector3 worldCoord = rotatedPos + placementOffset;

            GeneratedDoorData generatedDoorData = new GeneratedDoorData();
            generatedDoorData.generatedRoomData = this;
            generatedDoorData.localCoord = rotatedPos;
            generatedDoorData.worldCoord = worldCoord;
            generatedDoorData.doorDir = (DoorDir)Mod((int)doorData.doorDir + rot90Factor, 4);
            generatedDoorDatas.Add(generatedDoorData);
        }
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