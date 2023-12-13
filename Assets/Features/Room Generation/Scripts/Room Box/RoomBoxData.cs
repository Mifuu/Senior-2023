using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoxData : MonoBehaviour
{
    public enum DataType
    {
        Area = 0,
        DoorXPlus = 1,
        DoorXMinus = 2,
        DoorYPlus = 3,
        DoorYMinus = 4,
        DoorZPlus = 5,
        DoorZMinus = 6
    }
    public HashSet<Vector3Int> data = new HashSet<Vector3Int>();
}
