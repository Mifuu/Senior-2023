using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSet", menuName = "Room Generation/RoomSet")]
public class RoomSet : ScriptableObject
{
    public RoomData[] roomDatas;
}
