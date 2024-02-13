using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayUI;

public class PlayerUIUpdater : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerLevel playerLevel;

    GameplayUIReceiver gameplayUIReceiver;

    void Start()
    {
        gameplayUIReceiver = GameplayUIReceiver.Instance;

        if (playerHealth.IsOwner && gameplayUIReceiver)
        {
            playerHealth.currentHealth.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateHPUI(playerHealth.currentHealth.Value, playerHealth.maxHealth);
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }

        if (playerLevel.IsOwner && gameplayUIReceiver)
        {
            playerLevel.levelSystem.OnExpChange += (sender, arg) =>
            gameplayUIReceiver.UpdateExpUI(playerLevel.levelSystem.GetExpNormalized());
            playerLevel.levelSystem.OnLevelChange += (sender, arg) =>
            gameplayUIReceiver.UpdateLevelUI(playerLevel.levelSystem.GetLevel());

            gameplayUIReceiver.UpdateExpUI(playerLevel.levelSystem.GetExpNormalized());
            gameplayUIReceiver.UpdateLevelUI(playerLevel.levelSystem.GetLevel());
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }
    }
}
