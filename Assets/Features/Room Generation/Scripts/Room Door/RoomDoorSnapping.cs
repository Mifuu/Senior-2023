using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorSnapping : MonoBehaviour
{
    public static Vector3 snapOffset = new Vector3(0f, 0.5f, 0f);

    [Space]
    [Header("Debug")]
    public Vector3 _pos;
    void SnapPosition()
    {
        _pos = transform.position;
        _pos.x = (Mathf.Round(_pos.x / RoomBoxSnapping.snapValue.x - snapOffset.x) + snapOffset.x) * RoomBoxSnapping.snapValue.x;
        _pos.y = (Mathf.Round(_pos.y / RoomBoxSnapping.snapValue.y - snapOffset.y) + snapOffset.y) * RoomBoxSnapping.snapValue.y;
        _pos.z = (Mathf.Round(_pos.z / RoomBoxSnapping.snapValue.z - snapOffset.z) + snapOffset.z) * RoomBoxSnapping.snapValue.z;

        transform.position = _pos;
    }

    void OnDrawGizmosSelected()
    {
        SnapPosition();
    }
}
