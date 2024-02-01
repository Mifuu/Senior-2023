using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class PlayerLook : NetworkBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

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
            // check if the Alt key is being held down
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                return;
            }

            // calculate camera rotation for looking up and down
            xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            // apply this to our camera transformation
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            // rotate plyer to look left and right
            Vector3 rotationVector = Vector3.up * (mouseX * Time.deltaTime) * xSensitivity;
            transform.Rotate(rotationVector);
        }
    }
}