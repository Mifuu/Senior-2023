using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{
    public class RoomDoorDataGetter : MonoBehaviour
    {
        [Space]
        [ReadOnly]
        public RoomBoxSnapValue snapValue;

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
            _coord.x /= snapValue.value.x;
            _coord.y /= snapValue.value.y;
            _coord.z /= snapValue.value.z;
            roomDoorData.AddData(_coord, roomDoorSnapping.doorDir);

            return roomDoorData;
        }
    }
}