using System.Collections;
using System.Collections.Generic;
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
            roomSet.roomSetItems = new RoomSetItem[0];

            string[] guids = AssetDatabase.FindAssets("t:RoomData", new[] { roomSet.roomDataPath });

            Debug.Log("Found " + guids.Length + " asset in " + roomSet.roomDataPath + " folder");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                RoomData roomData = AssetDatabase.LoadAssetAtPath<RoomData>(path);

                RoomSetItem roomSetItem = new RoomSetItem();
                roomSetItem.roomData = roomData;

                ArrayUtility.Add(ref roomSet.roomSetItems, roomSetItem);
            }
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