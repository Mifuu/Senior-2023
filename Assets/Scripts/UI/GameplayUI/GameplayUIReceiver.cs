using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayUI
{
    [RequireComponent(typeof(GameplayUIManager))]
    public class GameplayUIReceiver : MonoBehaviour
    {
        public GameplayUIManager manager;
        GameplayUIPlayPanel playPanel;

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
    }
}