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
        public TextMeshProUGUI upgradeSkillCardText;
        public Image gunSlotImage_1;
        public Image gunSlotImage_2;
        public Image gunSlotImage_3;

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

        public void UpdateGunSlot(Sprite gunImage_1, Sprite gunImage_2, Sprite gunImage_3)
        {
            Debug.Log("GameplayUI: Update Gun Slot");
            gunSlotImage_1.sprite = gunImage_1;
            gunSlotImage_2.sprite = gunImage_2;
            gunSlotImage_3.sprite = gunImage_3;
        }
    }
}