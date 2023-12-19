using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataPlotter : MonoBehaviour
{
    static readonly Color EDITOR_COLOR = new Color(0.5f, 1f, 1f, 1f);

    [Space]
    public RoomData roomData;

    [Space]
    [Header("roomData Info")]
    [ReadOnly]
    [SerializeField]
    private List<RoomBoxSnapping> roomBoxSnappings;
    [SerializeField]
    private List<RoomDoorSnapping> roomDoorSnappings;

    [Space]
    [Header("Requirements")]
    public Transform roomBoxesParent;
    public Transform roomDoorsParent;

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
    [HideInInspector]
    public string latestRoomDoorData = "";

    public void AddRoomDoor()
    {
        if (roomDoorsParent == null)
        {
            Debug.LogError("RoomDataPlotter.AddRoomDoor(): roomDoorsParent is null.");
            return;
        }

        // game object setup
        GameObject newDoor = new GameObject("_Room Door");
        newDoor.transform.parent = roomDoorsParent;
        newDoor.transform.localPosition = Vector3.zero;
        newDoor.transform.localRotation = Quaternion.identity;
        newDoor.transform.localScale = Vector3.one;

        // box collider data and snapping setup
        newDoor.AddComponent<RoomDoorSnapping>();
        newDoor.AddComponent<RoomDoorDataGetter>();

        UpdateRoomDoor();
    }

    [ContextMenu("UpdateRoomDoor")]
    public void UpdateRoomDoor()
    {
        if (roomDoorsParent == null)
        {
            Debug.LogError("RoomDataPlotter.GetColliders(): dataColliderParent is null.");
            return;
        }

        roomDoorSnappings = new List<RoomDoorSnapping>();

        foreach (Transform t in roomDoorsParent)
        {
            if (t.TryGetComponent(out RoomDoorSnapping roomDoorSnapping))
            {
                roomDoorSnappings.Add(roomDoorSnapping);
            }
        }
    }

    [ContextMenu("AddRoomBox")]
    public void AddRoomBox()
    {
        if (roomBoxesParent == null)
        {
            Debug.LogError("RoomDataPlotter.AddCollider(): dataColliderParent is null.");
            return;
        }

        // game object setup
        GameObject newCollider = new GameObject("_Room Box");
        newCollider.transform.parent = roomBoxesParent;
        newCollider.transform.localPosition = Vector3.zero;
        newCollider.transform.localRotation = Quaternion.identity;
        newCollider.transform.localScale = Vector3.one;

        // box collider setup
        var b = newCollider.AddComponent<BoxCollider>();
        b.size = new Vector3(3, 3, 3);

        // box collider data and snapping setup
        var s = newCollider.AddComponent<RoomBoxSnapping>();
        newCollider.AddComponent<RoomBoxDataGetter>();
        s.boxCollider = b;

        UpdateRoomBox();
    }

    [ContextMenu("UpdateRoomBox")]
    public void UpdateRoomBox()
    {
        if (roomBoxesParent == null)
        {
            Debug.LogError("RoomDataPlotter.GetColliders(): dataColliderParent is null.");
            return;
        }

        roomBoxSnappings = new List<RoomBoxSnapping>();

        foreach (Transform t in roomBoxesParent)
        {
            if (t.TryGetComponent(out RoomBoxSnapping roomBoxSnapping))
            {
                roomBoxSnappings.Add(roomBoxSnapping);
            }
        }
    }

    [ContextMenu("GetRoomBoxData")]
    public void GetRoomBoxData()
    {
        // get roomBoxDataFromColliders
        List<RoomBoxDataGetter> roomBoxDataGetters = new List<RoomBoxDataGetter>();
        foreach (Transform t in roomBoxesParent)
        {
            if (t.TryGetComponent(out RoomBoxDataGetter g))
                roomBoxDataGetters.Add(g);
        }

        // get roomBoxData
        if (roomData == null)
        {
            Debug.Log("RoomDataPlotter.GetRoomBoxData(): roomData is null.");
            return;
        }
        roomData.roomBoxData.Clear();
        foreach (RoomBoxDataGetter roomBoxDataGetter in roomBoxDataGetters)
            roomData.roomBoxData.AddData(roomBoxDataGetter.GetRoomBoxData());

        // set debug output
        latestRoomBoxData = roomData.roomBoxData.ToGridString(16);
    }

    [ContextMenu("GetRoomDoorData")]
    public void GetRoomDoorData()
    {
        // get roomDoorDataFromColliders
        List<RoomDoorDataGetter> roomDoorDataGetters = new List<RoomDoorDataGetter>();
        foreach (Transform t in roomDoorsParent)
        {
            if (t.TryGetComponent(out RoomDoorDataGetter g))
                roomDoorDataGetters.Add(g);
        }

        // get room door data
        if (roomData == null)
        {
            Debug.Log("RoomDataPlotter.GetRoomDoorData(): roomData is null.");
            return;
        }
        roomData.roomDoorData.Clear();
        foreach (RoomDoorDataGetter g in roomDoorDataGetters)
            roomData.roomDoorData.AddData(g.GetRoomDoorData());

        // set debug output
        latestRoomDoorData = roomData.roomDoorData.ToString();
    }

    private void OnDrawGizmos()
    {
        transform.position = Vector3.zero;

        Color fillColor = new Color(EDITOR_COLOR.r, EDITOR_COLOR.g, EDITOR_COLOR.b, colliderDataFillVisibility);
        Color outlineColor = new Color(EDITOR_COLOR.r, EDITOR_COLOR.g, EDITOR_COLOR.b, colliderDataLineVisibility);

        foreach (var b in roomBoxSnappings)
        {
            if (b == null) continue;

            BoxCollider c = b.GetComponent<BoxCollider>();

            // draw fill
            Gizmos.color = fillColor;
            Gizmos.DrawCube(b.transform.position + c.center, c.size);


            if (showGrid)
            {
                // draw grid
                Gizmos.color = outlineColor;
                DrawGrid(c);
            }
            else
            {
                // draw outline
                Gizmos.color = outlineColor;
                Gizmos.DrawWireCube(b.transform.position + c.center, c.size);
            }
        }

        foreach (var d in roomDoorSnappings)
        {
            if (d == null) continue;

            // draw fill
            Gizmos.color = fillColor;
            Gizmos.DrawIcon(d.transform.position, "Packages/com.unity.collab-proxy/Editor/PlasticSCM/Assets/Images/d_iconbranch@2x.png", true);
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
