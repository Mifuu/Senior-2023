using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGenUtil;

public class RoomGenerator : MonoBehaviour
{
    public RoomSet roomSet;

    // Generation Data
    Dictionary<Vector3Int, int> roomGrid = new Dictionary<Vector3Int, int>();           // checking vacancy
    List<GameObject> roomObjects = new List<GameObject>();                              // storing game object references
    List<GeneratedRoomData> generatedRoomDatas = new List<GeneratedRoomData>();         // storing a;; generated room data
    List<GeneratedDoorData> generatedDoorDatas = new List<GeneratedDoorData>();         // storing all generated door data
    List<GeneratedDoorData> vacantDoorDatas = new List<GeneratedDoorData>();            // storing possible doors to spawn next room
    // List<DoorData> roomDoorDatas = new List<DoorData>();

    // Debug
    [Header("Debug")]
    public Vector3 latestTargetDoorPos;
    public bool isDebugMode = false;


    [Header("Generation Settings")]
    public int seed = 0;
    public int randDoorAttempts = 10;

    public void Clear()
    {
        roomGrid = new Dictionary<Vector3Int, int>();
        roomObjects = new List<GameObject>();
        generatedRoomDatas = new List<GeneratedRoomData>();
        vacantDoorDatas = new List<GeneratedDoorData>();

        // remove all children game objects
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public bool StepAddRoom()
    {
        // check for matching requirements
        foreach (var r in roomSet.roomSetItems.Select(i => i.roomData))
        {
            if (r.snapValue.value != roomSet.snapValue.value)
            {
                Debug.LogError("RoomGenerator.StepAddRoom(): roomSet.snapValue.value != roomData.snapValue.value");
                return false;
            }
        }

        // if is first room
        if (roomObjects.Count == 0)
        {
            // set or random seed
            if (seed == -1)
                Random.InitState(System.DateTime.Now.Millisecond);
            else
                Random.InitState(seed);

            if (isDebugMode) Debug.Log("Seed: " + Random.state);

            // get starting room
            RoomData roomData = roomSet.GetStartingRoomData();
            int rot90Factor = 0;
            if (roomData.enableRot90Variant)
                rot90Factor = Random.Range(0, 4);
            CandidateDoor firstDoor = new CandidateDoor(roomData.roomDoorData.doorDatas[0], rot90Factor);

            // add room
            AddRoom(firstDoor);

            // add doors

            return true;
        }

        // rand door and rand room to connect
        for (int i = 0; i < randDoorAttempts; i++)
        {
            // if no more doors available
            if (vacantDoorDatas.Count == 0)
                return false;

            // random door data
            GeneratedDoorData targetDoor = GetRandom(vacantDoorDatas);
            latestTargetDoorPos = V3Multiply(targetDoor.worldCoord, roomSet.snapValue.value);

            // get possible connection
            List<CandidateDoor> candidateDoors = GetCandidateDoors(targetDoor);

            // if no possible rooms for the specific door
            if (candidateDoors.Count == 0)
            {
                vacantDoorDatas.Remove(targetDoor);
                continue;
            }

            // choosing random possible rooms
            CandidateDoor connectingDoor = GetRandom(candidateDoors);

            // add room and add new doorData to the list
            AddRoom(connectingDoor, targetDoor);

            // remove doordata
            vacantDoorDatas.Remove(targetDoor);

            return true;
        }

        Debug.Log("Out of Attempts");
        return false;
    }

    void AddFirstRoom(RoomData roomData)
    {
        GameObject roomObject = Instantiate(roomData.roomPrefab, transform.position, Quaternion.identity, transform);
        roomObjects.Add(roomObject);
        int index = 0;
        int rot90Factor = 1;

        // add roomBoxData to roomGrid
        AddRoomToGrid(roomData, rot90Factor, Vector3Int.zero, index);

        // add GeneratedRoomData
        GeneratedRoomData generatedRoomData = new GeneratedRoomData(roomData, index, rot90Factor, roomObject, Vector3Int.zero);
        generatedRoomDatas.Add(generatedRoomData);

        foreach (GeneratedDoorData d in generatedRoomData.generatedDoorDatas)
        {
            vacantDoorDatas.Add(d);
        }

        if (roomObject.TryGetComponent(out RoomDataPlotter plotter))
            plotter.DisablePlotting();
    }

    GeneratedRoomData AddRoom(CandidateDoor candidateDoor, GeneratedDoorData targetDoor = null)
    {
        Vector3 candidateDoorCoord = RoomRotUtil.GetRot90Pos(candidateDoor.doorData.coord, candidateDoor.rot90Factor);
        Vector3Int placementOffset = GetPlacementOffset(candidateDoorCoord, (targetDoor != null) ? targetDoor.worldCoord : Vector3Int.zero);
        Vector3 realOffset = placementOffset;
        realOffset.x *= roomSet.snapValue.value.x;
        realOffset.y *= roomSet.snapValue.value.y;
        realOffset.z *= roomSet.snapValue.value.z;

        // add room game object
        GameObject roomObject = Instantiate(candidateDoor.doorData.roomData.roomPrefab, transform.position + realOffset, Quaternion.Euler(0, candidateDoor.rot90Factor * 90, 0), transform);
        roomObjects.Add(roomObject);
        int index = roomObjects.Count - 1;

        if (isDebugMode && targetDoor != null)
            Debug.Log(targetDoor.generatedRoomData.id + "->" + index);

        // add room to roomGrid
        AddRoomToGrid(candidateDoor, placementOffset, index);

        // add GeneratedRoomData
        GeneratedRoomData generatedRoomData = new GeneratedRoomData(candidateDoor.doorData.roomData, index, candidateDoor.rot90Factor, roomObject, placementOffset);
        generatedRoomDatas.Add(generatedRoomData);

        // add GeneratedDoorData to lists
        foreach (GeneratedDoorData d in generatedRoomData.generatedDoorDatas)
        {
            generatedDoorDatas.Add(d);

            // Don't add the new pair door to vacantDoorDatas but set the pair
            if (targetDoor != null && d.worldCoord == targetDoor.worldCoord)
            {   // pair doors
                d.SetPair(targetDoor);
                d.UpdateDoorObject();
                targetDoor.SetPair(d);
                targetDoor.UpdateDoorObject();
                continue;
            }
            else
            {   // add new non-pair doors to vacantDoorDatas
                vacantDoorDatas.Add(d);
            }
        }

        // disable plotting
        if (roomObject.TryGetComponent(out RoomDataPlotter plotter))
        {
            plotter.DisablePlotting();
        }

        roomObject.AddComponent<GeneratedRoomDataViewer>().generatedRoomData = generatedRoomData;

        return generatedRoomData;
    }

    void AddRoomToGrid(CandidateDoor candidateDoor, Vector3Int placementOffset, int index)
    {
        AddRoomToGrid(candidateDoor.doorData.roomData, candidateDoor.rot90Factor, placementOffset, index);
    }
    void AddRoomToGrid(RoomData roomData, int rot90Factor, Vector3Int placementOffset, int index)
    {
        foreach (Vector3Int pos in roomData.roomBoxData.roomSpaces)
        {
            Vector3Int _pos = RoomRotUtil.GetRot90Grid(pos, rot90Factor) + placementOffset;
            roomGrid.Add(_pos, index);

            if (isDebugMode) Debug.Log("AddRoomToGrid: " + _pos + " " + index);
        }
    }

    T GetRandom<T>(List<T> list) => list[Random.Range(0, list.Count)];

    T GetRandom<T>(T[] array) => array[Random.Range(0, array.Length)];


    /// <summary>Get all doors that's compatible with the target door</summary>
    public List<CandidateDoor> GetCandidateDoors(GeneratedDoorData targetDoor)
    {
        // add all doors with compatible parameters
        List<CandidateDoor> candidateDoors = GetCandidateDoors(targetDoor.doorDir);

        // Debug.Log("candidateDoors 1: " + candidateDoors.Count);

        string logText = "";
        foreach (var d in candidateDoors)
        {
            logText += d.doorData.roomData.name + "\n";
        }

        // filter out room that can't be place
        for (int i = candidateDoors.Count - 1; i >= 0; i--)
        {
            CandidateDoor d = candidateDoors[i];
            if (!CheckVacancy(d, targetDoor))
                candidateDoors.Remove(d);
        }

        // Debug.Log("candidateDoors 2: " + candidateDoors.Count + "_________________________________________" + "\n" + logText);

        return candidateDoors;
    }

    /// <summary>Get all doors with the specific parameters</summary>
    // note - maybe should cache in the future?
    public List<CandidateDoor> GetCandidateDoors(DoorDir targetDir)
    {
        DoorDir connectingDir = GetConnectingDoorDir(targetDir);

        List<CandidateDoor> candidateDoors = new List<CandidateDoor>();

        // filter 1: add rooms with compatible door direction inclusive of rot90
        foreach (RoomData roomData in roomSet.GetRoomDatas())
        {
            if (!roomData.enableRot90Variant)
            {   // without rot90, only added those with exact direction
                foreach (DoorData d in roomData.roomDoorData.doorDatas)
                {
                    if (d.doorDir == connectingDir)
                        candidateDoors.Add(new CandidateDoor(d, 0));
                }
            }
            else
            {   // with rot90, all doors is compatible after doing the necessary rotation
                foreach (DoorData d in roomData.roomDoorData.doorDatas)
                {
                    int rot90 = Mod(connectingDir - d.doorDir, 4);
                    candidateDoors.Add(new CandidateDoor(d, rot90));
                }
            }
        }

        // filter 2: remove rooms without tag compatibility

        // output
        return candidateDoors;
    }

    public DoorDir GetConnectingDoorDir(DoorDir targetDoorDir)
    {
        switch (targetDoorDir)
        {
            case DoorDir.ZPos:
                return DoorDir.ZNeg;
            case DoorDir.XPos:
                return DoorDir.XNeg;
            case DoorDir.ZNeg:
                return DoorDir.ZPos;
            case DoorDir.XNeg:
                return DoorDir.XPos;
        }

        Debug.LogError("RoomGenerator.GetConnectingDoorDir(): invalid targetDoorDir.");
        return DoorDir.ZPos;
    }

    public bool CheckVacancy(CandidateDoor candidateDoor, GeneratedDoorData targetDoor)
    {
        // rotate candidateDoor to match rot90
        Vector3 candidateDoorCoord = RoomRotUtil.GetRot90Pos(candidateDoor.doorData.coord, candidateDoor.rot90Factor);
        Vector3Int placementOffset = GetPlacementOffset(candidateDoorCoord, targetDoor.worldCoord);

        // check if all roomGrid are vacant
        foreach (Vector3Int pos in candidateDoor.doorData.roomData.roomBoxData.roomSpaces)
        {
            Vector3Int _pos = RoomRotUtil.GetRot90Grid(pos, candidateDoor.rot90Factor) + placementOffset;
            if (roomGrid.ContainsKey(_pos))
            {
                return false;
            }
        }

        return true;
    }

    public Vector3Int GetPlacementOffset(DoorData connectingDoor, DoorData targetDoor)
    {
        Vector3 _p = targetDoor.coord - connectingDoor.coord;
        Vector3Int placementOffset = new Vector3Int(Mathf.RoundToInt(_p.x), Mathf.RoundToInt(_p.y), Mathf.RoundToInt(_p.z));
        return placementOffset;
    }

    public Vector3Int GetPlacementOffset(Vector3 connectingDoorCoord, Vector3 targetDoorCoord)
    {
        Vector3 _p = targetDoorCoord - connectingDoorCoord;
        Vector3Int placementOffset = new Vector3Int(Mathf.RoundToInt(_p.x), Mathf.RoundToInt(_p.y), Mathf.RoundToInt(_p.z));
        return placementOffset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var d in vacantDoorDatas)
        {
            Vector3 _p = d.worldCoord;
            _p.x *= roomSet.snapValue.value.x;
            _p.y *= roomSet.snapValue.value.y;
            _p.z *= roomSet.snapValue.value.z;
            Gizmos.DrawSphere(_p, 0.5f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(latestTargetDoorPos, 0.7f);
    }

    Vector3 V3Multiply(Vector3 v, Vector3 i)
    {
        v.x *= i.x;
        v.y *= i.y;
        v.z *= i.z;
        return v;
    }

    int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
