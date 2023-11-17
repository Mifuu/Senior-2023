using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerMotor : NetworkBehaviour
{
    private CharacterController controller;
    private Rigidbody playerRigidBody;
    private Vector3 playerVelocity;
    private float speed = 20f;
    private float gravity = -9.8f;
    private bool isGrounded;
    private float jumpHeight = 4.0f;

    [SerializeField] private Vector2 defaultPositionRange = new Vector2(-4, -4);

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 2,
            Random.Range(defaultPositionRange.x, defaultPositionRange.y));
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2.0f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}