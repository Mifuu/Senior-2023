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
        public TextMeshProUGUI promptText;
        public TextMeshProUGUI keyText;
        public TextMeshProUGUI waterShardText;
        public TextMeshProUGUI fireShardText;
        public TextMeshProUGUI earthShardText;
        public TextMeshProUGUI windShardText;
        public TextMeshProUGUI upgradeSkillCardText;

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

        public void UpdatePromptText(string text)
        {
            promptText.text = text;
        }

        public void UpdateUpgradeSkillCardText(int point)
        {
            if (point == 0)
                upgradeSkillCardText.text = "";
            else
                upgradeSkillCardText.text = $"[T] Choose Skill Card({point})";
        }

        public void UpdateKeyText(int keyValue)
        {
            keyText.text = "Key: " + keyValue.ToString();
        }

        public void UpdateWaterShardText(int Value)
        {
            waterShardText.text = "Water Shard: " + Value.ToString();
        }
        public void UpdateFireShardText(int Value)
        {
            fireShardText.text = "Fire Shard: " + Value.ToString();
        }
        public void UpdateEarthShardText(int Value)
        {
            earthShardText.text = "Earth Shard: " + Value.ToString();
        }
        public void UpdateWindShardText(int Value)
        {
            windShardText.text = "Wind Shard: " + Value.ToString();
        }
    }
}