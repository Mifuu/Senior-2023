using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomDataPlotter : MonoBehaviour
{
    static readonly Color EDITOR_COLOR = new Color(0.5f, 1f, 1f, 1f);
    [HideInInspector]
    public Color editorColor = EDITOR_COLOR;

    [Header("Editting Settings")]
    [SerializeField]
    [ReadOnly]
    private bool isPlotting = true;
    public bool IsPlotting { get { return isPlotting; } }

    [Space]
    [Header("Requirements")]
    public bool enableRot90Variant = false;
    public RoomData roomData;
    public RoomBoxSnapValue snapValue;

    [Space]
    [Header("roomData Info")]
    [ReadOnly]
    [SerializeField]
    public List<GameObject> roomBoxes;
    [ReadOnly]
    [SerializeField]
    public List<GameObject> roomDoors;

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
    [Range(0f, 1f)]
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
        var _s = newDoor.AddComponent<RoomDoorSnapping>();
        _s.snapValue = snapValue;
        var _dg = newDoor.AddComponent<RoomDoorDataGetter>();
        _dg.snapValue = snapValue;
        newDoor.AddComponent<RoomDoorObject>();

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

        roomDoors = new List<GameObject>();

        foreach (Transform t in roomDoorsParent)
        {
            if (t.TryGetComponent(out RoomDoorSnapping roomDoorSnapping))
            {
                roomDoors.Add(roomDoorSnapping.gameObject);
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
        var _b = newCollider.AddComponent<BoxCollider>();
        _b.size = new Vector3(3, 3, 3);

        // box collider data and snapping setup
        var _s = newCollider.AddComponent<RoomBoxSnapping>();
        _s.boxCollider = _b;
        _s.snapValue = snapValue;
        var _dg = newCollider.AddComponent<RoomBoxDataGetter>();
        _dg.snapValue = snapValue;

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

        roomBoxes = new List<GameObject>();

        foreach (Transform t in roomBoxesParent)
        {
            if (t.TryGetComponent(out RoomBoxSnapping roomBoxSnapping))
            {
                roomBoxes.Add(roomBoxSnapping.gameObject);
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

        // set data
        roomData.roomBoxData.Clear();
        foreach (RoomBoxDataGetter roomBoxDataGetter in roomBoxDataGetters)
            roomData.roomBoxData.AddData(roomBoxDataGetter.GetRoomBoxData());
        roomData.enableRot90Variant = enableRot90Variant;
        roomData.snapValue = snapValue;

        // set debug output
        latestRoomBoxData = roomData.roomBoxData.ToGridString(16);

        EditorUtility.SetDirty(roomData);
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

        // set data
        roomData.roomDoorData.Clear();
        foreach (RoomDoorDataGetter g in roomDoorDataGetters)
            roomData.roomDoorData.AddData(g.GetRoomDoorData(roomData));
        roomData.enableRot90Variant = enableRot90Variant;
        roomData.snapValue = snapValue;

        // set debug output
        latestRoomDoorData = roomData.roomDoorData.ToString();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(roomData);
    }

    private void OnDrawGizmos()
    {
        if (isPlotting)
            transform.position = Vector3.zero;

        Color fillColor = new Color(editorColor.r, editorColor.g, editorColor.b, colliderDataFillVisibility);
        Color outlineColor = new Color(editorColor.r, editorColor.g, editorColor.b, colliderDataLineVisibility);

        foreach (var b in roomBoxes)
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

        foreach (var d in roomDoors)
        {
            if (d == null) continue;
            RoomDoorSnapping _d = d.GetComponent<RoomDoorSnapping>();

            // draw fill
            Gizmos.color = fillColor;
            Gizmos.DrawIcon(d.transform.position, "Packages/com.unity.collab-proxy/Editor/PlasticSCM/Assets/Images/d_iconbranch@2x.png", true);

            // draw arrow
            Gizmos.color = outlineColor;
            _d.DrawArrow();

            if (!_d.gameObject.TryGetComponent(out RoomDoorObject roomDoorObject))
            {
                _d.gameObject.AddComponent<RoomDoorObject>();
            }
        }

        // snapValue Update
        // foreach (var d in roomDoors)
        // {
        //     if (d == null) continue;
        //     RoomDoorSnapping _d = d.GetComponent<RoomDoorSnapping>();
        //     _d.snapValue = snapValue;
        //     RoomDoorDataGetter _dg = d.GetComponent<RoomDoorDataGetter>();
        //     _dg.snapValue = snapValue;
        // }
        // foreach (var b in roomBoxes)
        // {
        //     if (b == null) continue;
        //     RoomBoxSnapping _b = b.GetComponent<RoomBoxSnapping>();
        //     _b.snapValue = snapValue;
        //     RoomBoxDataGetter _bg = b.GetComponent<RoomBoxDataGetter>();
        //     _bg.snapValue = snapValue;
        // }
    }

    private void DrawGrid(BoxCollider b)
    {
        Bounds bounds = b.bounds;
        Vector3 min = bounds.min;

        // directional values
        Vector3 up = Vector3.up * bounds.size.y;
        Vector3 forward = Vector3.forward * bounds.size.z;
        Vector3 right = Vector3.right * bounds.size.x;

        // draw x ring
        for (Vector3 current = min; current.x <= bounds.max.x; current.x += snapValue.value.x)
        {
            Gizmos.DrawLine(current, current + up);
            Gizmos.DrawLine(current, current + forward);
            Gizmos.DrawLine(current + up, current + up + forward);
            Gizmos.DrawLine(current + forward, current + up + forward);
        }

        // draw y ring
        for (Vector3 current = min; current.y <= bounds.max.y; current.y += snapValue.value.y)
        {
            Gizmos.DrawLine(current, current + right);
            Gizmos.DrawLine(current, current + forward);
            Gizmos.DrawLine(current + right, current + right + forward);
            Gizmos.DrawLine(current + forward, current + right + forward);
        }

        // draw z ring
        for (Vector3 current = min; current.z <= bounds.max.z; current.z += snapValue.value.z)
        {
            Gizmos.DrawLine(current, current + right);
            Gizmos.DrawLine(current, current + up);
            Gizmos.DrawLine(current + right, current + right + up);
            Gizmos.DrawLine(current + up, current + right + up);
        }

    }

    public void DisablePlotting()
    {
        isPlotting = false;

        foreach (Transform t in roomBoxesParent)
        {
            if (t.TryGetComponent(out RoomBoxDataGetter roomBoxDataGetter))
            {
                DestroyImmediate(roomBoxDataGetter);
            }
            if (t.TryGetComponent(out RoomBoxSnapping roomBoxSnapping))
            {
                DestroyImmediate(roomBoxSnapping);
            }
        }

        foreach (Transform t in roomDoorsParent)
        {
            if (t.TryGetComponent(out RoomDoorDataGetter roomDoorDataGetter))
            {
                DestroyImmediate(roomDoorDataGetter);
            }
        }

        editorColor = RandHue(editorColor);
    }

    public Color RandHue(Color col)
    {
        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);
        h = Random.Range(0f, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}
