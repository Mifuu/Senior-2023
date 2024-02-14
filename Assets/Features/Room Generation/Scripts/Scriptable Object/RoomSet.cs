using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RoomGeneration
{
    [CreateAssetMenu(fileName = "RoomSet", menuName = "Room Generation/RoomSet")]
    public class RoomSet : ScriptableObject
    {
        public RoomBoxSnapValue snapValue;
        public bool useFirstEntryAsStartingRoom = false;
        public RoomSetItem[] roomSetItems;
        [HideInInspector] public string roomDataPath = "";

        public RoomData[] GetRoomDatas()
        {
            return roomSetItems.Select(i => i.roomData).ToArray();
        }

        public RoomData GetStartingRoomData()
        {
            if (useFirstEntryAsStartingRoom)
                return roomSetItems[0].roomData;
            var i = GetRandom(roomSetItems);
            return i.roomData;
        }

        T GetRandom<T>(List<T> list) => list[Random.Range(0, list.Count)];

        T GetRandom<T>(T[] array) => array[Random.Range(0, array.Length)];

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
    }
}