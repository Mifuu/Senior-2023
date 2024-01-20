using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JUtil;

// NOTE: this code is for quick testing only!
// It doesn't handle a lot of bad case
public class JCharacter : MonoBehaviour
{
    private Rigidbody rb;

    [Header("movement")]
    public float speed = 5f;

    [Header("jump")]
    public float jumpVelocity = 5f;
    public float gravity = 1;
    public LayerMask groundLayer;
    public bool onGround = false;

    [Header("Look")]
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    private float xRotation = 0f;

    Vector3 velocity;

    [Header("Requirements")]
    [SerializeField] public Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cam != null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Jump()
    {
        onGround = false;
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }

    public void ProcessMove(Vector2 input)
    {
        float xInput = input.x;
        float zInput = input.y;

        velocity = (transform.right * xInput * speed + transform.forward * zInput * speed) + transform.up * rb.velocity.y;

        if (!onGround)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = 0;

        rb.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (JCollision.CheckLayer(collision.gameObject.layer, groundLayer))
        {
            onGround = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (JCollision.CheckLayer(collision.gameObject.layer, groundLayer))
        {
            onGround = false;
        }
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // calculate cam rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // apply
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}