using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using GameplayUI;

public class SkillCardUI : MonoBehaviour
{
    private GameplayUIController gameplayUIController;

    [System.Serializable]
    public class CardSlotUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public CardSlotUI[] cardSlotUIs;
    public GameObject cardSlot_1;
    public GameObject cardSlot_2;
    public GameObject cardSlot_3;
    public static SkillCardUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        gameplayUIController = transform.parent.GetComponent<GameplayUIController>();
    }

    public void SetCardSlotUI(int slotIndex, string name, string description, Sprite image, UnityAction chooseCard)
    {
        cardSlotUIs[slotIndex].upgradeNameDisplay.text = name;
        cardSlotUIs[slotIndex].upgradeDescriptionDisplay.text = description;

        void chooseAndClosePanel()
        {
            chooseCard.Invoke(); // Give the chosen card to the player
            gameplayUIController.CloseSkillCardPanel(); // Close the panel
        }
        cardSlotUIs[slotIndex].upgradeButton.onClick.AddListener(chooseAndClosePanel);
        cardSlotUIs[slotIndex].upgradeIcon.sprite = image;
    }

    public void RemoveAllListeners()
    {
        foreach (var slot in cardSlotUIs)
        {
            Debug.Log("cardSlotUIs is: " + cardSlotUIs);
            Debug.Log(slot.upgradeButton);
            
            slot.upgradeButton.onClick.RemoveAllListeners();
        }
    }
    
}
