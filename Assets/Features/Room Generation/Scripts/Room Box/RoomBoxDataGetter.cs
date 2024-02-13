using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(RoomBoxSnapping))]
    public class RoomBoxDataGetter : MonoBehaviour
    {
        [Space]
        [ReadOnly]
        [TextArea(12, 12)]
        public string latestOutput = "";

        [Space]
        [ReadOnly]
        public RoomBoxSnapValue snapValue;

        [ContextMenu("GetRoomBoxData")]
        public RoomBoxData GetRoomBoxData()
        {
            // get components
            BoxCollider boxCollider;
            RoomBoxSnapping roomBoxSnapping;
            TryGetComponent(out boxCollider);
            TryGetComponent(out roomBoxSnapping);

            // prep
            RoomBoxData roomBoxData = new RoomBoxData();
            Bounds bounds = boxCollider.bounds;

            // get min snap pos
            Vector3 _minCoord = bounds.min;
            _minCoord.x /= snapValue.value.x;
            _minCoord.y /= snapValue.value.y;
            _minCoord.z /= snapValue.value.z;
            Vector3Int minCoord = Vector3Int.RoundToInt(_minCoord);

            // get coord scale
            Vector3 _coordScale = bounds.size;
            _coordScale.x /= snapValue.value.x;
            _coordScale.y /= snapValue.value.y;
            _coordScale.z /= snapValue.value.z;
            Vector3Int coordScale = Vector3Int.RoundToInt(_coordScale);

            // get room spaces
            for (int x = 0; x < coordScale.x; x++)
            {
                for (int y = 0; y < coordScale.y; y++)
                {
                    for (int z = 0; z < coordScale.z; z++)
                    {
                        Vector3Int pos = minCoord + new Vector3Int(x, y, z);
                        roomBoxData.AddData(pos);
                    }
                }
            }

            // set debug output
            latestOutput = roomBoxData.ToGridString();

            return roomBoxData;
        }
    }
}