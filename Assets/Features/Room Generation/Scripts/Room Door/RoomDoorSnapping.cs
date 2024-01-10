using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorSnapping : MonoBehaviour
{
    [Space]
    [ReadOnly]
    public RoomBoxSnapValue snapValue;

    [Space]
    public static Vector3 snapOffset = new Vector3(0f, 0.5f, 0f);

    [Space]
    [Header("Direction")]
    public DoorDir doorDir;

    [Space]
    [Header("Debug")]
    public Vector3 _pos;
    void SnapPosition()
    {
        _pos = transform.position;
        _pos.x = (Mathf.Round(_pos.x / snapValue.value.x - snapOffset.x) + snapOffset.x) * snapValue.value.x;
        _pos.y = (Mathf.Round(_pos.y / snapValue.value.y - snapOffset.y) + snapOffset.y) * snapValue.value.y;
        _pos.z = (Mathf.Round(_pos.z / snapValue.value.z - snapOffset.z) + snapOffset.z) * snapValue.value.z;

        transform.position = _pos;
    }

    void OnDrawGizmosSelected()
    {
        SnapPosition();
    }

    public void DrawArrow()
    {
        Vector3 arrowDir = Vector3.zero;
        switch (doorDir)
        {
            case DoorDir.XPos:
                arrowDir += Vector3.right * 1f;
                break;
            case DoorDir.XNeg:
                arrowDir += Vector3.left * 1f;
                break;
            case DoorDir.ZPos:
                arrowDir += Vector3.forward * 1f;
                break;
            case DoorDir.ZNeg:
                arrowDir += Vector3.back * 1f;
                break;
        }
        Gizmos.DrawRay(transform.position, arrowDir);

        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawSphere(transform.position + arrowDir * 0.6f, 0.3f);
        Gizmos.DrawSphere(transform.position + arrowDir, 0.2f);
    }
}
