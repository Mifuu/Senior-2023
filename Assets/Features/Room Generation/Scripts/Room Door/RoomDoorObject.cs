using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorObject : MonoBehaviour
{
    [Header("State")]
    public bool isWall = true;

    [Header("Game Object Settings")]
    public GameObject wallObject;
    public GameObject passageObject;

    public void Set(bool isWall)
    {
        this.isWall = isWall;
        if (wallObject != null) wallObject.SetActive(isWall);
        if (passageObject != null) passageObject?.SetActive(!isWall);
    }
}
