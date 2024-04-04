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
    private bool isLocked = false;

    private void Start()
    {
        //GameplayUI.GameplayUIStack.Instance.lockMouse.OnValueChanged += OnLockMouseChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            cam.enabled = false;
        }
    }

    void OnLockMouseChanged (bool previous, bool current)
    {
        isLocked = current;
    }

    public void ProcessLook(Vector2 input)
    {
        if (!IsClient || !IsOwner) return;

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

            if (!isLocked)
            {
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
}