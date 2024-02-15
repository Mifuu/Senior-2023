using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.Events;
using System;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    [field: SerializeField] public float maxHealth { get; set; }
    public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f);
    public event Action OnPlayerDie;

    private void Start()
    {
        // Initialize current health to max health on start
        currentHealth.Value = maxHealth;
    }

    public void Damage(float damageAmount, GameObject dealer)
    {
        if (!IsServer) return;

        currentHealth.Value -= damageAmount;

        if (currentHealth.Value <= 0f)
        {
            Die(dealer);
        }
    }
    public void Die(GameObject killer)
    {
        Debug.Log("Player Script: Player has died!");
        if (OnPlayerDie != null)
        {
            OnPlayerDie.Invoke();
        }
        Respawn();
    }

    private void Respawn()
    {
        currentHealth.Value = maxHealth;
        transform.position = new Vector3(0f, 0f, 0f);
    }
}
