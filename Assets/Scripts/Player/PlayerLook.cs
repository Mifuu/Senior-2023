using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

public class PlayerLook : NetworkBehaviour
{
    public Camera cam;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    private float xRotation = 0f;

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            cam.enabled = false;
        }
    }

    public void ProcessLook(Vector2 input)
    {
        if (!IsClient || !IsOwner)
            return;

        float mouseX = input.x;
        float mouseY = input.y;

        if (IsClient && !IsOwner)
        {
            cam.enabled = false;
        }

        if (IsClient && IsOwner)
        {
            xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            Vector3 rotationVector = Vector3.up * (mouseX * Time.deltaTime) * xSensitivity;
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.Rotate(rotationVector);
        }
    }
}