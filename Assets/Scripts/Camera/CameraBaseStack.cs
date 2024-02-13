using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraBaseStack : MonoBehaviour
{
    public static CameraBaseStack Instance { get; private set; }
    [HideInInspector] public Camera baseCamera;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        baseCamera = GetComponent<Camera>();
    }

    public void AddSubCamera(Camera subCamera)
    {
        var cameraData = baseCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Insert(0, subCamera);
    }
}
