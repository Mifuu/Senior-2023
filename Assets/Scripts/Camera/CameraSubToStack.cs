using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraSubToStack : MonoBehaviour
{
    Camera subCamera;

    void Start()
    {
        subCamera = GetComponent<Camera>();
        CameraBaseStack.Instance.AddSubCamera(subCamera);
    }
}
