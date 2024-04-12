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
    private bool isRefreshing = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<PlayerMotor>();
        RecalculateTotalDashCount();
        dashBuffAdditionBefore = DashBuffAddition.Value;
        currentDashCount = maxDashCount;
        DashBuffAddition.OnValueChanged += (prev, current) => RecalculateTotalDashCount();
    }

    public int GetMaxDashCount()
    {
        return maxDashCount;
    }

    private void RecalculateTotalDashCount()
    {
        maxDashCount = baseDashCount + DashBuffAddition.Value;
        InitializedDashBuffAdditionChanged();
    }

    private void InitializedDashBuffAdditionChanged()
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

    // Continuously refill the currentDashCount until it reached maxDashCount
    private IEnumerator DashCooldown()
    {
        currentDashCount--;
        if (isRefreshing) yield return 0;
        isRefreshing = true;
        while (currentDashCount < maxDashCount)
        {
            yield return new WaitForSeconds(dashCooldown);
            currentDashCount++;
        }
        isRefreshing = false;
  
        if (currentDashCount > maxDashCount )
        {
            currentDashCount = maxDashCount; // Cap the dash count to the max dash count
        }
    }
}

