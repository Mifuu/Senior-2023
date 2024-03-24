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
    [SerializeField] private float dashCooldown = 5.0f;
    [SerializeField] private int baseDashCount = 1;
    [SerializeField] public NetworkVariable<int> DashBuffAddition { get; set; } = new NetworkVariable<int>(0);
    private int dashBuffAdditionBefore;
    public int maxDashCount = 1;
    public int currentDashCount;
    private bool isDashing = false;
    private bool isOnCooldown = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<PlayerMotor>();
        RecalculateTotalDashCount();
        dashBuffAdditionBefore = DashBuffAddition.Value;
        currentDashCount = maxDashCount;
        DashBuffAddition.OnValueChanged += (prev, current) => RecalculateTotalDashCount();
    }

    private void RecalculateTotalDashCount()
    {
        maxDashCount = baseDashCount + DashBuffAddition.Value;
        InitializedDashBuddAdditionChanged();
    }

    private void InitializedDashBuddAdditionChanged()
    {
        int dashBuffAdditionDifference = DashBuffAddition.Value - dashBuffAdditionBefore;
        currentDashCount += dashBuffAdditionDifference;
    }

    public void Dash(Vector2 input)
    {
        if (isDashing || currentDashCount == 0 || !IsOwner) return;
        Vector3 dashDirection = CalculateDashDirection(input);
        StartCoroutine(PerformDash(dashDirection));
        StartCoroutine(DashCooldown());
        currentDashCount--;
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
        currentDashCount++; // Increment the dash count after cooldown
        if (currentDashCount > maxDashCount)
        {
            currentDashCount = maxDashCount; // Cap the dash count to the max dash count
        }
    }
}

