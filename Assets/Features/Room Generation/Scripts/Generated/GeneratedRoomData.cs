using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoomGenUtil;

namespace RoomGeneration
{
    [System.Serializable]
    public class GeneratedRoomData
    {
        [Header("Init Data")]
        public int id;
        public RoomData roomData;
        public int rot90Factor = 0;
        public Vector3Int placementOffset;
        public GameObject gameObject;

        [Header("Generated Data")]
        public List<Vector3Int> roomSpaces;
        public List<GeneratedDoorData> generatedDoorDatas;

        [Header("Plotter Data")]
        RoomDataPlotter roomDataPlotter;
        public Transform roomBoxesParent;
        public Transform roomDoorsParent;
        public List<GameObject> roomBoxes;
        public List<GameObject> roomDoors;

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

            // set room data plotter
            if (gameObject.TryGetComponent<RoomDataPlotter>(out RoomDataPlotter roomDataPlotter))
            {
                this.roomDataPlotter = roomDataPlotter;
                roomBoxesParent = roomDataPlotter.roomBoxesParent;
                roomDoorsParent = roomDataPlotter.roomDoorsParent;
                roomDoors = new List<GameObject>(roomDataPlotter.roomDoors);
                roomBoxes = new List<GameObject>(roomDataPlotter.roomBoxes);
            }
            else
            {
                Debug.Log("RoomDataPlotter not found.");
            }
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
            for (int i = 0; i < roomData.roomDoorData.doorDatas.Count; i++)
            {
                DoorData doorData = roomData.roomDoorData.doorDatas[i];

                Vector3 rotatedPos = RoomRotUtil.GetRot90Pos(doorData.coord, rot90Factor);
                Vector3 worldCoord = rotatedPos + placementOffset;

                // set generated door data
                DoorDir doorDir = (DoorDir)Mod((int)doorData.doorDir + rot90Factor, 4);
                RoomDoorObject _roomDoorObject = gameObject.transform.GetComponentsInChildren<RoomDoorObject>().ToList()[i];
                GeneratedDoorData generatedDoorData = new GeneratedDoorData(this, doorData.coord, rotatedPos, worldCoord, doorDir, _roomDoorObject);
                generatedDoorDatas.Add(generatedDoorData);
            }

            // update RoomDoorObjects
            foreach (GeneratedDoorData d in generatedDoorDatas)
            {
                d.UpdateDoorObject();
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
        [System.NonSerialized]
        public GeneratedRoomData generatedRoomData;
        public Vector3 initCoord;
        public Vector3 localCoord;
        public Vector3 worldCoord;
        public DoorDir doorDir;

        [System.NonSerialized]
        public GeneratedDoorData pairedDoorData;

        public RoomDoorObject roomDoorObject;

        public GeneratedDoorData(GeneratedRoomData generatedRoomData, Vector3 initCoord, Vector3 localCoord, Vector3 worldCoord, DoorDir doorDir, RoomDoorObject roomDoorObject)
        {
            this.generatedRoomData = generatedRoomData;
            this.initCoord = initCoord;
            this.localCoord = localCoord;
            this.worldCoord = worldCoord;
            this.doorDir = doorDir;
            this.roomDoorObject = roomDoorObject;
        }

        public void SetPair(GeneratedDoorData pairedDoorData)
        {
            this.pairedDoorData = pairedDoorData;
            pairedDoorData.pairedDoorData = this;
        }

        public void UpdateDoorObject()
        {
            if (pairedDoorData == null)
                roomDoorObject.Set(true);
            else
                roomDoorObject.Set(false);
        }
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
}