using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayUI;
using TMPro;
using JetBrains.Annotations;

public class PlayerUIUpdater : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerInventory playerInventory;
    private PlayerSwitchWeapon gunHolder;
    
    GameplayUIReceiver gameplayUIReceiver;

    public void InitializePlayerSwitchWeapon()
    {
        gunHolder = transform.parent.GetComponentInChildren<PlayerSwitchWeapon>();
    }

    void Start()
    {
        gameplayUIReceiver = GameplayUIReceiver.Instance;

        // Update Player Health
        if (playerHealth.IsOwner && gameplayUIReceiver)
        {
            playerHealth.currentHealth.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateHPUI(playerHealth.currentHealth.Value, playerHealth.maxHealth);
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }

        // Update Player EXP & Level
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

        // Update Player Prompt Text
        if (playerInteract.IsOwner && gameplayUIReceiver)
        {
            playerInteract.OnPromptTextChanged += HandlePromptTextChanged =>
            gameplayUIReceiver.UpdatePromptText(playerInteract.promptText);
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }

        // Update Player Upgrade Skill Card Text
        if (playerInteract.IsOwner && gameplayUIReceiver)
        {
            playerLevel.levelSystem.OnSkillCardPointChange += (sender, arg) =>
            gameplayUIReceiver.UpdateUpgradeSkillCardText(playerLevel.levelSystem.GetSkillCardPoint());
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }

        // Update Player Inventory
        if (playerInventory != null && playerInventory.IsOwner && gameplayUIReceiver)
        {
            playerInventory.Key.OnValueChanged += (prev, current) => 
            gameplayUIReceiver.UpdateKeyText(playerInventory.Key.Value);

            playerInventory.WaterShard.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateWaterShardText(playerInventory.WaterShard.Value);

            playerInventory.FireShard.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateFireShardText(playerInventory.FireShard.Value);

            playerInventory.EarthShard.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateEarthShardText(playerInventory.EarthShard.Value);

            playerInventory.WindShard.OnValueChanged += (prev, current) =>
            gameplayUIReceiver.UpdateWindShardText(playerInventory.WindShard.Value);
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }
    }

    public void UpdateGunSprite(Sprite sprite_1, Sprite sprite_2, Sprite sprite_3)
    {
        Debug.Log("PlayerUIUpdater: Update");
        if (gunHolder != null && gunHolder.IsOwner && gameplayUIReceiver)
        {
            Debug.Log("PlayerUIUpdater: Update in IF");
            gameplayUIReceiver.UpdateGunSlotImage(sprite_1, sprite_2, sprite_3);
        }
        else if (!gameplayUIReceiver)
        {
            Debug.LogWarning("GameplayUIReceiver is not found");
        }
        else if (!gunHolder)
        {
            Debug.LogWarning("GunHolder is not found");
        }
    }

    public void UpdateSelectedGunSlotColor(int selectedGunIndex)
    {
        gameplayUIReceiver.UpdateSelectedGunSlotImage(selectedGunIndex);
    }
}
