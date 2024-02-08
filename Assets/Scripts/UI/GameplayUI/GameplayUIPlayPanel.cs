using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayUI
{
    public class GameplayUIPlayPanel : MonoBehaviour
    {
        public GameplayUIManager manager;

        [Header("Requirements")]
        public RelaySlider healthSlider;

        public void UpdateHP(float health, float maxHealth)
        {
            healthSlider.SetValue(health / maxHealth);
        }
    }
}