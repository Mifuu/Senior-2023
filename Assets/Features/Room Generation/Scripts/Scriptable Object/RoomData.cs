using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Room Generation/RoomData")]
public class RoomData : ScriptableObject
{
    public RoomBoxData roomBoxData;

    public RoomData()
    {
        roomBoxData = new RoomBoxData();
    }
}
