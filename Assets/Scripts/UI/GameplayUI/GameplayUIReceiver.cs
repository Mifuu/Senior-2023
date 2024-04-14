using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameplayUI
{
    [RequireComponent(typeof(GameplayUIManager))]
    public class GameplayUIReceiver : MonoBehaviour
    {
        [SerializeField] private GameplayUIManager manager;
        [SerializeField] private GameplayUIPlayPanel playPanel;
        [SerializeField] private GameplayUIInventoryPanel inventoryPanel;

        public static GameplayUIReceiver Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            playPanel = manager.playPanel;
        }

        public void UpdateHPUI(float health, float maxHealth)
        {
            playPanel.UpdateHP(health, maxHealth);
        }

        public void UpdateExpUI(float exp)
        {
            playPanel.UpdateExp(exp);
        }

        public void UpdateLevelUI(int level)
        {
            playPanel.UpdateLevel(level);
        }

        public void UpdatePromptText(string text)
        {
            playPanel.UpdatePromptText(text);
        }

        public void UpdateUpgradeSkillCardText(int point)
        {
            playPanel.UpdateUpgradeSkillCardText(point);
        }

        /*
        public void UpdateKeyText(int keyValue)
        {
            playPanel.UpdateKeyText(keyValue);
        }

        public void UpdateWaterShardText(int Value)
        {
            playPanel.UpdateWaterShardText(Value);
        }

        public void UpdateFireShardText(int Value)
        {
            playPanel.UpdateFireShardText(Value);
        }
        public void UpdateEarthShardText(int Value)
        {
            playPanel.UpdateEarthShardText(Value);
        }
        public void UpdateWindShardText(int Value)
        {
            playPanel.UpdateWindShardText(Value);
        }
        */

        public void UpdateKeyText(int keyValue)
        {
            inventoryPanel.UpdateKeyText(keyValue);
        }

        public void UpdateWaterShardText(int Value)
        {
            inventoryPanel.UpdateWaterShardText(Value);
        }

        public void UpdateFireShardText(int Value)
        {
            inventoryPanel.UpdateFireShardText(Value);
        }
        public void UpdateEarthShardText(int Value)
        {
            inventoryPanel.UpdateEarthShardText(Value);
        }
        public void UpdateWindShardText(int Value)
        {
            inventoryPanel.UpdateWindShardText(Value);
        }

        public void UpdateGunSlotImage(Sprite gunSprite_1, Sprite gunSprite_2, Sprite GunSprite_3)
        {
            Debug.Log("GameplayUIReceiver: Update");
            playPanel.UpdateGunSlot(gunSprite_1, gunSprite_2, GunSprite_3);
        }
    }
}