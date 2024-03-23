using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SkillCardUI : MonoBehaviour
{
    SkillCard[] skillCards = new SkillCard[8];
    public CardSlotUI[] cardSlotUIs;
    [System.Serializable]
    public class CardSlotUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public static SkillCardUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetCardSlotUI(int slotIndex, string name, string description, Sprite image, UnityAction call)
    {
        cardSlotUIs[slotIndex].upgradeNameDisplay.text = name;
        cardSlotUIs[slotIndex].upgradeDescriptionDisplay.text = description;
        cardSlotUIs[slotIndex].upgradeButton.onClick.AddListener(call);
        cardSlotUIs[slotIndex].upgradeIcon.sprite = image;
    }

    public void RemoveAllListeners()
    {
        foreach (var slot in cardSlotUIs)
        {
            slot.upgradeButton.onClick.RemoveAllListeners();
        }
    }
    
}
