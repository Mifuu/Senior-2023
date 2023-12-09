using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataPlotter : MonoBehaviour
{
    static readonly Color EDITOR_COLOR = new Color(0.5f, 1f, 1f, 1f);
    RoomData roomData;

    [Space]
    [ReadOnly]
    [TextArea(2, 4)]
    public string note1 = "This script is used to plot the room data in the scene view.\n" +
                   "Require gizmos to be turned on in the scene view.";
    [Range(0f, 1f)]
    public float colliderDataVisibility = 0.15f;

    [Space]
    [Header("roomData Info")]
    [ReadOnly]
    [SerializeField]
    private List<BoxCollider> colliders;

    [Space]
    [Header("Requirements")]
    public Transform dataColliderParent;

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
        newCollider.AddComponent<BoxColliderData>();
        var s = newCollider.AddComponent<BoxColliderSnapping>();
        s.boxCollider = b;

        UpdateColliderData();
    }

    private void OnDrawGizmos()
    {
        Color fillColor = new Color(EDITOR_COLOR.r, EDITOR_COLOR.g, EDITOR_COLOR.b, colliderDataVisibility);
        Color outlineColor = EDITOR_COLOR;

        foreach (var b in colliders)
        {
            if (b == null) continue;

            // draw fill
            Gizmos.color = fillColor;
            Gizmos.DrawCube(b.transform.position + b.center, b.size);

            // draw outline
            Gizmos.color = outlineColor;
            Gizmos.DrawWireCube(b.transform.position + b.center, b.size);
        }
    }
}
