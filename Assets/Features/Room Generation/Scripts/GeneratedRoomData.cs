using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedRoomData
{
    public RoomData roomData;
    public int rot90Factor = 0;
    public Vector3Int placementOffset;

    public GameObject gameObject;
    public List<Vector3Int> roomSpaces;
    public List<GeneratedDoorData> roomDoorDatas;

    public GeneratedRoomData(RoomData roomData, int rot90Factor, Vector3Int placementOffset)
    {
        this.roomData = roomData;
        this.rot90Factor = rot90Factor;
        this.placementOffset = placementOffset;

        roomSpaces = new List<Vector3Int>();
        roomDoorDatas = new List<GeneratedDoorData>();
    }
}

public class GeneratedDoorData
{
    public GeneratedRoomData generatedRoomData;
    public Vector3 localCoord;
    public Vector3 coord;
    public DoorDir doorDir;
}