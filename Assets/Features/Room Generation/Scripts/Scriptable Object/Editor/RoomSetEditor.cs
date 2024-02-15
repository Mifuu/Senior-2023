using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

namespace RoomGeneration
{
    [CustomEditor(typeof(RoomSet))]
    public class RoomSetEditor : Editor
    {
        RoomSet roomSet;
        NetworkPrefabsList networkPrefabsList;

        public override void OnInspectorGUI()
        {
            RoomSet roomSet = (RoomSet)target;

            YEditorUtility.BestGirlBanner();

            GUILayout.Label("Editor Tools");
            roomSet.roomDataPath = EditorGUILayout.TextField("Path: ", roomSet.roomDataPath);
            GUILayout.Space(10);
            GUILayout.Label("Auto Loading Assets from folder");
            if (GUILayout.Button("Load Assets"))
                LoadAssets();

            GUILayout.Space(10);
            GUILayout.Label("Auto Setting Same Name Prefabs");
            if (GUILayout.Button("Get Room Prefabs"))
                GetRoomPrefabs();

            GUILayout.Space(25);
            DrawDefaultInspector();
        }

        void LoadAssets()
        {
            RoomSet roomSet = (RoomSet)target;
            // roomSet.roomSetItems = new RoomSetItem[0];

            string[] guids = AssetDatabase.FindAssets("t:RoomData", new[] { roomSet.roomDataPath });
            Debug.Log("Found " + guids.Length + " asset in " + roomSet.roomDataPath + " folder");

            // create new list
            List<RoomSetItem> newList = new List<RoomSetItem>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                RoomData roomData = AssetDatabase.LoadAssetAtPath<RoomData>(path);

                RoomSetItem roomSetItem = new RoomSetItem();
                roomSetItem.roomData = roomData;

                newList.Add(roomSetItem);
            }

            List<RoomSetItem> currentList = new List<RoomSetItem>(roomSet.roomSetItems);
            List<RoomData> newRoomDatas = newList.Select(i => i.roomData).ToList();

            // remove items that is in current list but not in new list from current list
            for (int i = currentList.Count - 1; i >= 0; i--)
            {
                if (!newRoomDatas.Contains(currentList[i].roomData))
                {
                    currentList.RemoveAt(i);
                }
            }

            List<RoomData> currentRoomDatas = currentList.Select(i => i.roomData).ToList();

            // add items that is in new list but not in current list to current list
            foreach (RoomSetItem n in newList)
            {
                if (!currentRoomDatas.Contains(n.roomData))
                {
                    currentList.Add(n);
                }
            }

            roomSet.roomSetItems = currentList.ToArray();
        }

        void GetRoomPrefabs()
        {
            RoomSet roomSet = (RoomSet)target;
            foreach (RoomSetItem roomSetItem in roomSet.roomSetItems)
            {
                // load prefab to room data with the same name
                roomSetItem.roomData.roomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(roomSet.roomDataPath + "/" + roomSetItem.roomData.name + ".prefab");

                Debug.Log(roomSet.roomDataPath + "/" + roomSetItem.roomData.name + ".prefab");

                // EditorUtility.SetDirty(roomSetItem.roomData);
            }
        }
    }
}