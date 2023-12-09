using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class BoxColliderSnapping : MonoBehaviour
{
    [SerializeField] public BoxCollider boxCollider;
    [SerializeField] private float snapValue = 2f;

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

        center.x = Mathf.Round(center.x / snapValue) * snapValue;
        center.y = Mathf.Round(center.y / snapValue) * snapValue;
        center.z = Mathf.Round(center.z / snapValue) * snapValue;

        size.x = Mathf.Round(size.x / snapValue) * snapValue;
        size.y = Mathf.Round(size.y / snapValue) * snapValue;
        size.z = Mathf.Round(size.z / snapValue) * snapValue;

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
        if (boxCollider.size.x % snapValue != 0)
        {
            size.x = Mathf.Round(size.x / snapValue) * snapValue;
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
        else if (boxCollider.size.y % snapValue != 0)
        {
            size.y = Mathf.Round(size.y / snapValue) * snapValue;
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
        else if (boxCollider.size.z % snapValue != 0)
        {
            size.z = Mathf.Round(size.z / snapValue) * snapValue;
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
    }

    void OnDrawGizmosSelected()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();

        SnapBoxCollider2();
    }
}
