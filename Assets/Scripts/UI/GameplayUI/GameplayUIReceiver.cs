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

        public void UpdateKeyText(int keyValue)
        {
            playPanel.UpdateKeyText(keyValue);
        }
    }
}