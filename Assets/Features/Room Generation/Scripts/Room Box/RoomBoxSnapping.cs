using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class RoomBoxSnapping : MonoBehaviour
{
    [SerializeField] public BoxCollider boxCollider;

    [Space]
    [ReadOnly]
    public RoomBoxSnapValue snapValue;


    [Space]
    [Header("Debug")]
    public Vector3 _center;
    public Vector3 _size;

    void Awake()
    {
        if (boxCollider == null) return;
        _center = boxCollider.center;
        _size = boxCollider.size;
    }

    void SnapBoxCollider()
    {
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;

        center.x = Mathf.Round(center.x / snapValue.value.x) * snapValue.value.x;
        center.y = Mathf.Round(center.y / snapValue.value.y) * snapValue.value.y;
        center.z = Mathf.Round(center.z / snapValue.value.z) * snapValue.value.z;

        size.x = Mathf.Round(size.x / snapValue.value.x) * snapValue.value.x;
        size.y = Mathf.Round(size.y / snapValue.value.y) * snapValue.value.y;
        size.z = Mathf.Round(size.z / snapValue.value.z) * snapValue.value.z;

        boxCollider.center = center;
        boxCollider.size = size;
    }

    void SnapBoxCollider2()
    {
        // determine editting side
        Vector3 editDir = Vector3.zero;
        if (boxCollider.center.x - _center.x > 0) editDir.x = 1f;
        else if (boxCollider.center.x - _center.x < 0) editDir.x = -1f;
        else if (boxCollider.center.y - _center.y > 0) editDir.y = 1f;
        else if (boxCollider.center.y - _center.y < 0) editDir.y = -1f;
        else if (boxCollider.center.z - _center.z > 0) editDir.z = 1f;
        else if (boxCollider.center.z - _center.z < 0) editDir.z = -1f;

        // snap size and center accordingly to the editting side
        Vector3 size = boxCollider.size;
        if (boxCollider.size.x % snapValue.value.x != 0)
        {
            size.x = Mathf.Round(size.x / snapValue.value.x) * snapValue.value.x;
            boxCollider.size = size;

            // get absolute size diff
            Vector3 sizeDiff = boxCollider.size - _size;
            sizeDiff.x = Mathf.Abs(sizeDiff.x);
            sizeDiff.y = Mathf.Abs(sizeDiff.y);
            sizeDiff.z = Mathf.Abs(sizeDiff.z);

            // set center accordingly to the editting side
            if (editDir.x < 0)
                boxCollider.center = _center - sizeDiff / 2f;
            else
                boxCollider.center = _center + sizeDiff / 2f;

            _size = boxCollider.size;
            _center = boxCollider.center;
        }
        else if (boxCollider.size.y % snapValue.value.y != 0)
        {
            size.y = Mathf.Round(size.y / snapValue.value.y) * snapValue.value.y;
            boxCollider.size = size;

            // get absolute size diff
            Vector3 sizeDiff = boxCollider.size - _size;
            sizeDiff.x = Mathf.Abs(sizeDiff.x);
            sizeDiff.y = Mathf.Abs(sizeDiff.y);
            sizeDiff.z = Mathf.Abs(sizeDiff.z);

            // set center accordingly to the editting side
            if (editDir.y < 0)
                boxCollider.center = _center - sizeDiff / 2f;
            else
                boxCollider.center = _center + sizeDiff / 2f;

            _size = boxCollider.size;
            _center = boxCollider.center;
        }
        else if (boxCollider.size.z % snapValue.value.z != 0)
        {
            size.z = Mathf.Round(size.z / snapValue.value.z) * snapValue.value.z;
            boxCollider.size = size;

            // get absolute size diff
            Vector3 sizeDiff = boxCollider.size - _size;
            sizeDiff.x = Mathf.Abs(sizeDiff.x);
            sizeDiff.y = Mathf.Abs(sizeDiff.y);
            sizeDiff.z = Mathf.Abs(sizeDiff.z);

            // set center accordingly to the editting side
            if (editDir.z < 0)
                boxCollider.center = _center - sizeDiff / 2f;
            else
                boxCollider.center = _center + sizeDiff / 2f;

            _size = boxCollider.size;
            _center = boxCollider.center;
        }

        RevalidateBoxCenter();
    }

    [ContextMenu("Revalidate Box Size")]
    public void RevalidateBoxCenter()
    {
        _center = boxCollider.center;
        if (_size.x / snapValue.value.x % 2 == 0)
            _center.x = (Mathf.Round(_center.x / snapValue.value.x) * snapValue.value.x);
        else
            _center.x = ((Mathf.Round(_center.x / snapValue.value.x - 0.5f) + 0.5f) * snapValue.value.x);
        if (_size.y / snapValue.value.y % 2 == 0)
            _center.y = (Mathf.Round(_center.y / snapValue.value.y) * snapValue.value.y);
        else
            _center.y = ((Mathf.Round(_center.y / snapValue.value.y - 0.5f) + 0.5f) * snapValue.value.y);
        if (_size.z / snapValue.value.z % 2 == 0)
            _center.z = (Mathf.Round(_center.z / snapValue.value.z) * snapValue.value.z);
        else
            _center.z = ((Mathf.Round(_center.z / snapValue.value.z - 0.5f) + 0.5f) * snapValue.value.z);
        boxCollider.center = _center;
    }

    void OnDrawGizmosSelected()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();

        SnapBoxCollider2();

        if (transform.position != Vector3.zero)
            transform.position = Vector3.zero;
    }
}
