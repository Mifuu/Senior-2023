using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorDataGetter : MonoBehaviour
{
    [ContextMenu("GetRoomDoorData")]
    public RoomDoorData GetRoomDoorData(RoomData parentRoom)
    {
        // get components
        RoomDoorSnapping roomDoorSnapping;
        TryGetComponent(out roomDoorSnapping);

        // prep
        RoomDoorData roomDoorData = new RoomDoorData(parentRoom);

        // get door coord
        Vector3 _coord = transform.position;
        _coord.x /= RoomBoxSnapping.snapValue.x;
        _coord.y /= RoomBoxSnapping.snapValue.y;
        _coord.z /= RoomBoxSnapping.snapValue.z;
        roomDoorData.AddData(_coord, roomDoorSnapping.doorDir);

        return roomDoorData;
    }
}
