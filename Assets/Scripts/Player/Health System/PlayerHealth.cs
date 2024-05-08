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
    private bool isDead = false;
    public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f);
    public event Action OnPlayerDie;

    private void Start()
    {
        if (!IsOwner) return;
        // Initialize current health to FinalMaxHealth on start
        maxHealth = BaseMaxHealth.Value;
        InitializePlayerHealthServerRpc();
        RecalculateFinalMaxHealthServerRpc();
        // Recalculate maxhealth everytime BaseMaxHealth or HealthBuffMultiplier values changed
        BaseMaxHealth.OnValueChanged += (prev, current) => RecalculateFinalMaxHealthServerRpc();
        HealthBuffMultiplier.OnValueChanged += (prev, current) => RecalculateFinalMaxHealthServerRpc();

    }

    [ServerRpc]
    private void InitializePlayerHealthServerRpc()
    {
        currentHealth.Value = BaseMaxHealth.Value;
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
        if (!IsServer) return;

        if (isDead) return;

        currentHealth.Value -= damageAmount;

        if (currentHealth.Value <= 0f)
        {
            Die(dealer);
        }

        BloodVignetteVFX.SimpleBloodVignette();
    }

    public void Die(GameObject killer)
    {
        Debug.Log("Player Script: Player has died!");
        OnPlayerDie?.Invoke();
        isDead = true;
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
        isDead = false;
        currentHealth.Value = maxHealth;

        if (TryGetComponent<PlayerManager>(out var playerManager))
        {
            if (MultiplayerGameManager.Instance)
            {
                MultiplayerGameManager.Instance.RespawnPlayerServerRpc(NetworkObject.OwnerClientId);
            }
            else
            {
                playerManager.TeleportToSpawnPoint();
            }
        }
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
