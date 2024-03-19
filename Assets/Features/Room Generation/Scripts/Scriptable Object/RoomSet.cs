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
        public RoomSetConnectMatrix connectMatrix;
        public float minPlayerSpawnRoomDistance = 40f;

        public List<RoomData> GetNormalRoomDatas()
        {
            return GetRoomDatasByTag(RoomTag.NormalRoom);
        }

        public RoomData GetOneRoomDataByTag(RoomTag tag)
        {
            List<RoomData> rooms = GetRoomDatasByTag(tag);
            if (rooms.Count() != 0)
            {
                return GetRandom(rooms);
            }

            Debug.Log($"RoomSet.GetOneRoomDataByTag: No room with {tag} tag found, returning random room.");
            var i = GetRandom(roomSetItems);
            return i.roomData;
        }

        public List<RoomData> GetRoomDatasByTag(RoomTag tag)
        {
            return roomSetItems.Where(i => i.HasTag(tag)).Select(i => i.roomData).ToList();
        }

        public RoomTag[] GetPreviousRoomTagsByRoomTag(RoomTag roomTag)
        {
            foreach (var item in connectMatrix.roomSetConnectMatrixItems)
            {
                if (item.tag == roomTag)
                    return item.CanSpawnFrom.ToArray();
            }

            Debug.LogError($"RoomSet.GetPreviousRoomTagsByRoomTag: No roomSetConnectMatrixItem found with {roomTag} tag.");
            return new RoomTag[0];
        }

        public RoomTag[] GetRoomTagsByRoomData(RoomData roomData)
        {
            return roomSetItems.Where(i => i.roomData == roomData).Select(i => i.GetAllTags()).FirstOrDefault().ToArray();
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
        [GenericMask("NormalRoom", "CenterRoom", "PlayerSpawnRoom", "MiniBossRoom")]
        [SerializeField] public int mask = 0;

        public bool HasTag(RoomTag tag)
        {
            return (mask & (1 << (int)tag)) != 0;
        }

        private static RoomTag[] allTags = new RoomTag[0];
        private static bool isAllTagsSet = false;
        public static RoomTag[] AllTags
        {
            get
            {
                if (!isAllTagsSet)
                {
                    allTags = Enum.GetValues(typeof(RoomTag)).Cast<RoomTag>().ToArray();
                    isAllTagsSet = true;
                }
                return allTags;
            }
        }
        public List<RoomTag> GetAllTags()
        {
            List<RoomTag> tags = new List<RoomTag>();
            foreach (RoomTag tag in AllTags)
            {
                if (HasTag(tag))
                    tags.Add(tag);
            }
            return tags;
        }
    }

    public enum RoomTag
    {
        NormalRoom,
        CenterRoom,
        PlayerSpawnRoom,
        MiniBossRoom,
    }

    [System.Serializable]
    public class RoomSetConnectMatrixItem
    {
        public RoomTag tag;
        public List<RoomTag> CanSpawnFrom;

        public bool HasTag(RoomTag tag)
        {
            return CanSpawnFrom.Contains(tag);
        }
    }

    /*
    [System.Serializable]
    public class RoomSetItem
    {
        public RoomData roomData;
        [GenericMask("NormalRoom", "CenterRoom", "PlayerSpawnRoom", "MiniBossRoom")]
        [SerializeField] public int mask = 0;

        public bool HasTag(RoomTag tag)
        {
            return (mask & (1 << (int)tag)) != 0;
        }
    }
    */
}