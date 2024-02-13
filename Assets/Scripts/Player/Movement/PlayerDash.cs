using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDash : NetworkBehaviour
{
    private CharacterController controller;
    private PlayerMotor motor;

    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    private bool isDashing = false;
    private bool isOnCooldown = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<PlayerMotor>();
    }

    public void Dash(Vector2 input)
    {
        if (isDashing || isOnCooldown || !IsOwner) return;
        Vector3 dashDirection = CalculateDashDirection(input);
        StartCoroutine(PerformDash(dashDirection));
        StartCoroutine(DashCooldown());
    }

    // Use player input from PlayerMotor to calculate dash direction
    private Vector3 CalculateDashDirection(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        moveDirection = transform.TransformDirection(moveDirection).normalized;
        return moveDirection;
    }

    private IEnumerator PerformDash(Vector3 dashDirection)
    {
        isDashing = true;
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            // Move the player with increased speed in the calculated dash direction
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    private IEnumerator DashCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        isOnCooldown = false;
    }
}