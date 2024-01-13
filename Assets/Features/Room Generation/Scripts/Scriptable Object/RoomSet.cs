using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "RoomSet", menuName = "Room Generation/RoomSet")]
public class RoomSet : ScriptableObject
{
    public RoomBoxSnapValue snapValue;
    public RoomSetItem[] roomSetItems;
    [HideInInspector] public string roomDataPath = "";

    public RoomData[] GetRoomDatas()
    {
        return roomSetItems.Select(i => i.roomData).ToArray();
    }

    public RoomData GetStartingRoomData()
    {
        var i = GetRandom(roomSetItems);
        return i.roomData;
    }

    T GetRandom<T>(List<T> list) => list[Random.Range(0, list.Count)];

    T GetRandom<T>(T[] array) => array[Random.Range(0, array.Length)];

    void OnValidate()
    {
        EditorUtility.SetDirty(this);
    }
}

[System.Serializable]
public class RoomSetItem
{
    public RoomData roomData;
}