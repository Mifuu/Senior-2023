using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public RoomSet roomSet;

    // Generation Data
    Dictionary<Vector3Int, int> roomGrid = new Dictionary<Vector3Int, int>();
    List<GameObject> roomObjects = new List<GameObject>();
    List<DoorData> roomDoorDatas = new List<DoorData>();

    // Debug
    public Vector3 latestTargetDoorCoord;

    // Generation Settings
    public int randDoorAttempts = 10;

    public void Clear()
    {
        roomGrid = new Dictionary<Vector3Int, int>();
        roomObjects = new List<GameObject>();
        roomDoorDatas = new List<DoorData>();

        // remove all children
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public bool StepAddRoom()
    {
        // if is first room
        if (roomObjects.Count == 0 && roomDoorDatas.Count == 0)
        {
            RoomData roomData = GetRandom(roomSet.roomDatas);

            AddFirstRoom(roomData);
            return true;
        }

        // rand door and rand room to connect
        for (int i = 0; i < randDoorAttempts; i++)
        {
            // if no more doors available
            if (roomDoorDatas.Count == 0)
                return false;

            // random door data
            DoorData targetDoor = GetRandom(roomDoorDatas);
            latestTargetDoorCoord = V3Multiply(targetDoor.coord, RoomBoxSnapping.snapValue);

            // random room
            List<DoorData> possibleConnectingDoors = GetPossibleConnectingDoor(targetDoor);

            // if no possible rooms for the specific door
            if (possibleConnectingDoors.Count == 0)
            {
                Debug.Log("REMOVE DOOR");
                roomDoorDatas.Remove(targetDoor);
                StepAddRoom();
                continue;
            }

            Debug.Log(possibleConnectingDoors.Count);

            // choosing random possible rooms
            DoorData connectingDoor = GetRandom(possibleConnectingDoors);

            // add room and add new doorData to the list
            AddRoom(connectingDoor, targetDoor);

            // remove doordata
            roomDoorDatas.Remove(targetDoor);

            return true;
        }

        Debug.Log("Out of Attempts");
        return false;
    }

    void AddFirstRoom(RoomData roomData)
    {
        GameObject r = Instantiate(roomData.roomPrefab, transform.position, Quaternion.identity, transform);
        roomObjects.Add(r);
        int index = 0;

        // add roomBoxData to roomGrid
        AddRoomToGrid(roomData, Vector3Int.zero, index);

        // add roomDoorData
        foreach (DoorData d in roomData.roomDoorData.doorDatas)
        {
            // new data diff from original prefab data
            DoorData _d = new DoorData(d);

            roomDoorDatas.Add(_d);
        }

        if (r.TryGetComponent(out RoomDataPlotter plotter))
            plotter.DisablePlotting();
    }

    void AddRoom(DoorData connectingDoor, DoorData targetDoor)
    {
        Vector3Int placementOffset = GetPlacementOffset(connectingDoor, targetDoor);
        Vector3 realOffset = placementOffset;
        realOffset.x *= RoomBoxSnapping.snapValue.x;
        realOffset.y *= RoomBoxSnapping.snapValue.y;
        realOffset.z *= RoomBoxSnapping.snapValue.z;

        // add room
        GameObject r = Instantiate(connectingDoor.parentRoom.roomPrefab, transform.position + realOffset, Quaternion.identity, transform);
        roomObjects.Add(r);
        int index = roomObjects.Count - 1;

        // add roomBoxData to roomGrid
        AddRoomToGrid(connectingDoor.parentRoom, placementOffset, index);

        // add roomDoorData
        foreach (DoorData d in connectingDoor.parentRoom.roomDoorData.doorDatas)
        {
            // skip connecting door
            if (d == connectingDoor) continue;

            // new data diff from original prefab data
            DoorData _d = new DoorData(d);
            _d.coord += placementOffset;

            roomDoorDatas.Add(_d);
        }

        if (r.TryGetComponent(out RoomDataPlotter plotter))
            plotter.DisablePlotting();
    }

    void AddRoomToGrid(RoomData roomData, Vector3Int placementOffset, int index)
    {
        foreach (Vector3Int pos in roomData.roomBoxData.roomSpaces)
        {
            Vector3Int _pos = pos + placementOffset;
            roomGrid.Add(_pos, index);
        }
    }

    T GetRandom<T>(List<T> list) => list[Random.Range(0, list.Count)];

    T GetRandom<T>(T[] array) => array[Random.Range(0, array.Length)];


    public List<DoorData> GetPossibleConnectingDoor(DoorData targetDoor)
    {
        List<DoorData> possibleDoors = new List<DoorData>();

        // add all doors with compatible direction
        DoorData.DoorDir connectingDir = GetConnectingDoorDir(targetDoor.doorDir);
        foreach (RoomData roomData in roomSet.roomDatas)
        {
            foreach (DoorData d in roomData.roomDoorData.doorDatas)
            {
                if (d.doorDir == connectingDir)
                    possibleDoors.Add(d);
            }
        }

        // filter out room that can't be place
        for (int i = possibleDoors.Count - 1; i >= 0; i--)
        {
            DoorData d = possibleDoors[i];
            if (!CheckVacancy(d, targetDoor))
                possibleDoors.Remove(d);
        }

        return possibleDoors;
    }

    public DoorData.DoorDir GetConnectingDoorDir(DoorData.DoorDir targetDoorDir)
    {
        switch (targetDoorDir)
        {
            case DoorData.DoorDir.ZPos:
                return DoorData.DoorDir.ZNeg;
            case DoorData.DoorDir.XPos:
                return DoorData.DoorDir.XNeg;
            case DoorData.DoorDir.ZNeg:
                return DoorData.DoorDir.ZPos;
            case DoorData.DoorDir.XNeg:
                return DoorData.DoorDir.XPos;
        }

        Debug.LogError("RoomGenerator.GetConnectingDoorDir(): invalid targetDoorDir.");
        return DoorData.DoorDir.ZPos;
    }

    public bool CheckVacancy(DoorData connectingDoor, DoorData targetDoor)
    {
        // get placement offset
        Vector3Int placementOffset = GetPlacementOffset(connectingDoor, targetDoor);

        // check if all roomGrid are vacant
        foreach (Vector3Int pos in connectingDoor.parentRoom.roomBoxData.roomSpaces)
        {
            Vector3Int _pos = pos + placementOffset;
            if (roomGrid.ContainsKey(_pos))
                return false;
        }

        return true;
    }

    public Vector3Int GetPlacementOffset(DoorData connectingDoor, DoorData targetDoor)
    {
        Vector3 _p = targetDoor.coord - connectingDoor.coord;
        Vector3Int placementOffset = new Vector3Int(Mathf.RoundToInt(_p.x), Mathf.RoundToInt(_p.y), Mathf.RoundToInt(_p.z));
        return placementOffset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var d in roomDoorDatas)
        {
            Vector3 _p = d.coord;
            _p.x *= RoomBoxSnapping.snapValue.x;
            _p.y *= RoomBoxSnapping.snapValue.y;
            _p.z *= RoomBoxSnapping.snapValue.z;
            Gizmos.DrawSphere(_p, 0.5f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(latestTargetDoorCoord, 0.7f);
    }

    Vector3 V3Multiply(Vector3 v, Vector3 i)
    {
        v.x *= i.x;
        v.y *= i.y;
        v.z *= i.z;
        return v;
    }
}
