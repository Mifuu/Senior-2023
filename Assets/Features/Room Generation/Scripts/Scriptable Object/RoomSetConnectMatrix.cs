using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{

    [CreateAssetMenu(fileName = "RoomSetConnectMatrix", menuName = "Room Generation/RoomSetConnectMatrix")]
    public class RoomSetConnectMatrix : ScriptableObject
    {
        public List<RoomSetConnectMatrixItem> roomSetConnectMatrixItems = new List<RoomSetConnectMatrixItem>();

        private static int roomTagCount = 0;
        private static bool setRoomTagCount = false;
        private static int RoomTagCount
        {
            get
            {
                if (!setRoomTagCount)
                {
                    roomTagCount = System.Enum.GetValues(typeof(RoomTag)).Length;
                    setRoomTagCount = true;
                }
                return roomTagCount;
            }
        }

        void OnValidate()
        {
            for (int i = 0; i < RoomTagCount; i++)
            {
                if (roomSetConnectMatrixItems.Count <= i)
                    roomSetConnectMatrixItems.Add(new RoomSetConnectMatrixItem());

                if (roomSetConnectMatrixItems[i] == null)
                    roomSetConnectMatrixItems[i] = new RoomSetConnectMatrixItem();

                if (roomSetConnectMatrixItems[i].tag != (RoomTag)i)
                    roomSetConnectMatrixItems[i].tag = (RoomTag)i;
            }

            for (int i = RoomTagCount; i < roomSetConnectMatrixItems.Count; i++)
            {
                roomSetConnectMatrixItems[i] = null;
            }

            for (int i = roomSetConnectMatrixItems.Count - 1; i >= RoomTagCount; i--)
            {
                roomSetConnectMatrixItems.RemoveAt(i);
            }
        }
    }
}