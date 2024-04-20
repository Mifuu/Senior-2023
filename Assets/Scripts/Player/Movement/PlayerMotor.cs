using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerMotor : NetworkBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float speed = 14f;
    private float gravity = -24f;
    private bool isGrounded;
    private float jumpHeight = 1.5f;

    [SerializeField] private Vector2 defaultPositionRange = new Vector2(-4, -4);

    private GameplayUI.GameplayUIStack gameplayUIStack;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // random player spawn point
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 2,
        Random.Range(defaultPositionRange.x, defaultPositionRange.y));
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        // Check if the "Alt" key is held down
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || GameplayUI.GameplayUIStack.Instance.Peek() == GameplayUI.PanelType.SkillCard)
        {
            // Make the cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
        {
            // If "Alt" key is not held, lock the cursor again
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public float GetMovementSpeed()
    {
        return speed;
    }

    public void ProcessMove(Vector2 input)
    {
        if (!IsOwner) return;
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