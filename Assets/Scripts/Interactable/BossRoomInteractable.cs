using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class BossRoomInteractable : InteractableItem
{
    PlayerManager playerManager;
    PlayerInventory playerInventory;

    public CanvasGroup canvasGroup;
    public TMP_Text statusMessage;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        playerInventory = playerManager.gameObject.GetComponent<PlayerInventory>();
        playerInventory.Key.OnValueChanged += OnPlayerKeyUpdate;
        OnPlayerKeyUpdate(0, 0);
    }

    void FixedUpdate()
    {
        // if (!playerManager)
        //     playerManager = PlayerManager.Instance;
    }

    void OnPlayerKeyUpdate(int previousValue, int newValue)
    {
        statusMessage.text = "You have " + newValue + " keys";
    }

    protected override void Interact(ulong PlayerId)
    {
        Debug.Log("interacted with" + gameObject.name);

        if (playerInventory.Key.Value >= 3)
        {
            Debug.Log("TODO: Teleport To Boss Room");
        }
    }
}
