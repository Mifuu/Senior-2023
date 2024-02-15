using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using PropertyAttributes;

namespace RoomGeneration
{
    [CreateAssetMenu(fileName = "RoomSet", menuName = "Room Generation/RoomSet")]
    public class RoomSet : ScriptableObject
    {
        public RoomBoxSnapValue snapValue;
        public RoomSetItem[] roomSetItems;
        [HideInInspector] public string roomDataPath = "";

        [Header("Generation Settings")]
        public float minPlayerSpawnRoomDistance = 40f;

        public List<RoomData> GetNormalRoomDatas()
        {
            return GetRoomDatasByTag(RoomTag.NormalRoom);
        }

        public RoomData GetStartingRoomData()
        {
            List<RoomData> centerRooms = GetRoomDatasByTag(RoomTag.CenterRoom);
            if (centerRooms.Count() != 0)
            {
                return GetRandom(centerRooms);
            }

            Debug.Log("RoomSet.GetStartingRoomData: No room with CenterRoom tag found, returning random room.");
            var i = GetRandom(roomSetItems);
            return i.roomData;
        }

        public RoomData GetPlayerSpawnRoomData()
        {
            List<RoomData> playerSpawnRooms = GetRoomDatasByTag(RoomTag.PlayerSpawnRoom);
            if (playerSpawnRooms.Count() != 0)
            {
                return GetRandom(playerSpawnRooms);
            }

            Debug.Log("RoomSet.GetPlayerSpawnRoomData: No room with PlayerSpawnRoom tag found, returning random room.");
            var i = GetRandom(roomSetItems);
            return i.roomData;
        }

        public List<RoomData> GetRoomDatasByTag(RoomTag tag)
        {
            return roomSetItems.Where(i => i.HasTag(tag)).Select(i => i.roomData).ToList();
        }

        T GetRandom<T>(List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

        T GetRandom<T>(T[] array) => array[UnityEngine.Random.Range(0, array.Length)];

        void OnValidate()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }

    [System.Serializable]
    public class RoomSetItem
    {
        public RoomData roomData;
        [GenericMask("NormalRoom", "CenterRoom", "PlayerSpawnRoom")]
        [SerializeField] private int mask = 0;

        public bool HasTag(RoomTag tag)
        {
            return (mask & (1 << (int)tag)) != 0;
        }
    }

    public enum RoomTag
    {
        NormalRoom = 0,
        CenterRoom = 1,
        PlayerSpawnRoom = 2
    }
}