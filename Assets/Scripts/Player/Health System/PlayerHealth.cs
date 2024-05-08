using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.Events;
using System;
using GameplayUI;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    public static float respawnTime = 5.0f;
    [SerializeField] public float maxHealth { get; set; }
    [SerializeField] public NetworkVariable<float> BaseMaxHealth { get; set; } = new NetworkVariable<float>(10.0f);
    [SerializeField] public NetworkVariable<float> HealthBuffMultiplier { get; set; } = new NetworkVariable<float>(1.0f);

    private float _maxHealth;
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
    public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(10.0f);
    public event Action OnPlayerDie;

    private void Start()
    {
        if (!IsOwner) return;
        // Initialize current health to FinalMaxHealth on start
        maxHealth = BaseMaxHealth.Value;
        RecalculateFinalMaxHealthServerRpc();
        // Recalculate maxhealth everytime BaseMaxHealth or HealthBuffMultiplier values changed
        BaseMaxHealth.OnValueChanged += (prev, current) => RecalculateFinalMaxHealthServerRpc();
        HealthBuffMultiplier.OnValueChanged += (prev, current) => RecalculateFinalMaxHealthServerRpc();

    }

    [ServerRpc]
    private void RecalculateFinalMaxHealthServerRpc()
    {

        _maxHealth = maxHealth;
        maxHealth = BaseMaxHealth.Value * HealthBuffMultiplier.Value;
        currentHealth.Value += maxHealth - _maxHealth;
        Debug.Log("Player Health: " + "max health = " + maxHealth + " Multiplier = " + HealthBuffMultiplier.Value);
 
    }

    public void Damage(float damageAmount, GameObject dealer)
    {
        if (isDead.Value) return;

        if (IsServer)
            currentHealth.Value -= damageAmount;
        else
            ApplyDamageServerRpc(damageAmount);

        if (currentHealth.Value <= 0f)
        {
            DieClientRpc();

            if (IsServer)
                isDead.Value = true;
            else
                SetIsDeadServerRpc(true);
        }

        BloodVignetteVFX.SimpleBloodVignette();
    }

    [ServerRpc] 
    void ApplyDamageServerRpc(float damageAmount)
    {
        currentHealth.Value -= damageAmount;
    }

    [ServerRpc]
    void SetIsDeadServerRpc(bool value)
    {
        isDead.Value = value;
    }

    [ClientRpc]
    void DieClientRpc()
    {
        Die(null);
    }

    public void Die(GameObject obj)
    {
        Debug.Log("Player Script: Player has died!");
        OnPlayerDie?.Invoke();
        StartCoroutine(RespawnCR());
    }

    IEnumerator RespawnCR()
    {
        InputManager inputManager = GetComponent<InputManager>();

        // set panel
        GameplayUIController.Instance.RespawnTrigger(respawnTime);
        inputManager.EnableInput(false);

        yield return new WaitForSeconds(respawnTime);

        // respawn
        inputManager.EnableInput(true);
        SetIsDeadServerRpc(false);
        SetPlayerHealthServerRpc(maxHealth);

        MultiplayerGameManager.Instance.TeleportPlayerToSpawnServerRpc(NetworkObject.OwnerClientId);
    }

    [ServerRpc]
    void SetPlayerHealthServerRpc(float health)
    {
        currentHealth.Value = health;
    }


    public float GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
