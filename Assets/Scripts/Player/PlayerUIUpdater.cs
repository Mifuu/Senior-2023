using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayUI;

public class PlayerUIUpdater : MonoBehaviour
{
    public PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth.IsOwner && GameplayUIReceiver.Instance)
            playerHealth.currentHealth.OnValueChanged += (prev, current) => UpdateHP();
    }

    void UpdateHP()
    {
        if (playerHealth.IsOwner && GameplayUIReceiver.Instance)
            GameplayUIReceiver.Instance.UpdateHPUI(playerHealth.currentHealth.Value, playerHealth.maxHealth);
    }
}
