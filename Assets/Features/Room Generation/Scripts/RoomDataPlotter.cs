using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataPlotter : MonoBehaviour
{
    static readonly Color EDITOR_COLOR = new Color(0.5f, 1f, 1f, 1f);
    RoomData roomData;

    [ReadOnly]
    [TextArea(2, 4)]
    public string note1 = "This script is used to plot the room data in the scene view.\n" +
                   "Require gizmos to be turned on in the scene view.";

    [Space]
    [Header("roomData Info")]
    [ReadOnly]
    [SerializeField]
    private List<BoxCollider> colliders;

    [Space]
    [Header("Requirements")]
    public Transform dataColliderParent;

    [Space]
    [Header("Debug")]
    [Range(0f, 1f)]
    [SerializeField]
    private float colliderDataLineVisibility = 0.30f;
    [SerializeField]
    private float colliderDataFillVisibility = 0.15f;
    [SerializeField]
    private bool showGrid = true;
    [HideInInspector]
    public string latestRoomBoxData = "";

    [ContextMenu("AddCollider")]
    public void AddCollider()
    {
        if (dataColliderParent == null)
        {
            Debug.LogError("RoomDataPlotter.AddCollider(): dataColliderParent is null.");
            return;
        }

        // game object setup
        GameObject newCollider = new GameObject("_Data Collider");
        newCollider.transform.parent = dataColliderParent;
        newCollider.transform.localPosition = Vector3.zero;
        newCollider.transform.localRotation = Quaternion.identity;
        newCollider.transform.localScale = Vector3.one;

        // box collider setup
        var b = newCollider.AddComponent<BoxCollider>();
        b.size = new Vector3(3, 3, 3);

        // box collider data and snapping setup
        var s = newCollider.AddComponent<RoomBoxSnapping>();
        newCollider.AddComponent<RoomBoxDataFromCollider>();
        s.boxCollider = b;

        UpdateColliderData();
    }

    [ContextMenu("UpdateColliders")]
    public void UpdateColliderData()
    {
        if (dataColliderParent == null)
        {
            Debug.LogError("RoomDataPlotter.GetColliders(): dataColliderParent is null.");
            return;
        }

        colliders = new List<BoxCollider>();

        foreach (Transform t in dataColliderParent)
        {
            if (t.TryGetComponent(out BoxCollider boxCollider))
            {
                colliders.Add(boxCollider);
            }
        }
    }

    [ContextMenu("GetRoomBoxData")]
    public void GetRoomBoxData()
    {
        // get roomBoxDataFromColliders
        List<RoomBoxDataFromCollider> roomBoxDataFromColliders = new List<RoomBoxDataFromCollider>();
        foreach (Transform t in dataColliderParent)
        {
            if (t.TryGetComponent(out RoomBoxDataFromCollider roomBoxDataFromCollider))
                roomBoxDataFromColliders.Add(roomBoxDataFromCollider);
        }

        // get roomBoxData
        roomData = new RoomData();
        foreach (RoomBoxDataFromCollider roomBoxDataFromCollider in roomBoxDataFromColliders)
            roomData.roomBoxData.AddData(roomBoxDataFromCollider.GetRoomBoxData());

        // set debug output
        latestRoomBoxData = roomData.roomBoxData.ToGridString(16);
    }

    private void OnDrawGizmos()
    {
        Color fillColor = new Color(EDITOR_COLOR.r, EDITOR_COLOR.g, EDITOR_COLOR.b, colliderDataFillVisibility);
        Color outlineColor = new Color(EDITOR_COLOR.r, EDITOR_COLOR.g, EDITOR_COLOR.b, colliderDataLineVisibility);

        foreach (var b in colliders)
        {
            if (b == null) continue;

            // draw fill
            Gizmos.color = fillColor;
            Gizmos.DrawCube(b.transform.position + b.center, b.size);


            if (showGrid)
            {
                // draw grid
                Gizmos.color = outlineColor;
                DrawGrid(b);
            }
            else
            {
                // draw outline
                Gizmos.color = outlineColor;
                Gizmos.DrawWireCube(b.transform.position + b.center, b.size);
            }
        }
    }

    private void DrawGrid(BoxCollider b)
    {
        Bounds bounds = b.bounds;
        Vector3 min = bounds.min;
        Vector3 snapValue = RoomBoxSnapping.snapValue;

        // directional values
        Vector3 up = Vector3.up * bounds.size.y;
        Vector3 forward = Vector3.forward * bounds.size.z;
        Vector3 right = Vector3.right * bounds.size.x;

        // draw x ring
        for (Vector3 current = min; current.x <= bounds.max.x; current.x += snapValue.x)
        {
            Gizmos.DrawLine(current, current + up);
            Gizmos.DrawLine(current, current + forward);
            Gizmos.DrawLine(current + up, current + up + forward);
            Gizmos.DrawLine(current + forward, current + up + forward);
        }

        // draw y ring
        for (Vector3 current = min; current.y <= bounds.max.y; current.y += snapValue.y)
        {
            Gizmos.DrawLine(current, current + right);
            Gizmos.DrawLine(current, current + forward);
            Gizmos.DrawLine(current + right, current + right + forward);
            Gizmos.DrawLine(current + forward, current + right + forward);
        }

        // draw z ring
        for (Vector3 current = min; current.z <= bounds.max.z; current.z += snapValue.z)
        {
            Gizmos.DrawLine(current, current + right);
            Gizmos.DrawLine(current, current + up);
            Gizmos.DrawLine(current + right, current + right + up);
            Gizmos.DrawLine(current + up, current + right + up);
        }

    }
}
