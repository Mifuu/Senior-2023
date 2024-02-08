using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameplayUI
{
    public class GameplayUIPlayPanel : MonoBehaviour
    {
        public GameplayUIManager manager;

        [Header("Requirements")]
        public RelaySlider healthSlider;
        public RelaySlider expSlider;
        public TMP_Text levelText;

        public void UpdateHP(float health, float maxHealth)
        {
            healthSlider.SetValue(health / maxHealth);
        }

        public void UpdateExp(float exp)
        {
            expSlider.SetValue(exp);
        }

        public void UpdateLevel(int level)
        {
            levelText.text = "lv:" + level.ToString();
        }
    }
}