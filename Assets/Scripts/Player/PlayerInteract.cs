using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private Camera cam;
    private PlayerSwitchWeapon switchWeapon;
    private InputManager inputManager;

    // Define the event for when the prompt text changes
    public event Action<string> OnPromptTextChanged;

    private string _promptText;
    public string promptText
    {
        get { return _promptText; }
        private set
        {
            if (_promptText != value)
            {
                _promptText = value;
                // Raise the event when the prompt text changes
                OnPromptTextChanged?.Invoke(_promptText);
            }
        }
    }

    private void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
        promptText = string.Empty;
    }

    public void InitializePlayerSwitchWeapon()
    {
        switchWeapon = transform.GetComponentInChildren<PlayerSwitchWeapon>();
    }

    private void Update()
    {
        // Clear text when player is not looking at any interactable item
        promptText = string.Empty;

        // Change player's prompt text to be the interactable item that player is looking
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo, distance, mask))
        {   
            InteractableItem interactable = hitinfo.collider.GetComponent<InteractableItem>();
            if (interactable != null)
            {
                //Debug.Log("player is looking at: " + interactable.name);
                promptText = interactable.promptMessage;
                if (inputManager.onFoot.Interact.triggered)
                {
                    GameObject playerObject = transform.gameObject;
                    if (playerObject == null)
                    {
                        Debug.LogError("PlayerInteract Script: playerObject is null");
                        return;
                    }
                    interactable.BaseInteract(playerObject);
                }
            }  
        }
    }

    public void DropHoldingGun()
    {
        if (!IsOwner) return;
        if (switchWeapon.guns.Length == 1) return; // make player can't drop if has only 1 gun

        int currentGunIndex = switchWeapon.currentGunIndex.Value;
        if (switchWeapon.guns[currentGunIndex] != null)
        {
            if (switchWeapon.guns[currentGunIndex].CanShoot()) 
            {
                // calculate gun drop position
                Vector3 spawnPosition = transform.position + transform.forward * 1;
                Vector3 aimDir = (cam.transform.forward).normalized;
                Quaternion gunRotation = Quaternion.LookRotation(aimDir, Vector3.up);
                DropHoldingGunServerRpc(spawnPosition, gunRotation);
            }
        }
        else
        {
            Debug.LogError("PlayerInteract Script: switchWeapon.guns[currentGunIndex] is null");
        }  
    }

    [ServerRpc]
    private void DropHoldingGunServerRpc(Vector3 playerPosition, Quaternion playerRotation)
    {
        // destroy the gun that player is holding
        int currentGunIndex = switchWeapon.currentGunIndex.Value;
        NetworkObject gunToDestroy = switchWeapon.guns[currentGunIndex].NetworkObject;
        GameObject gunToDrop = switchWeapon.guns[currentGunIndex].gunInteractable.gameObject;
        gunToDestroy.transform.SetParent(null);
        gunToDestroy.Despawn(true);

        // spawn the counterpart of the gun infront of the player
        var gunObject = Instantiate(gunToDrop.gameObject, playerPosition, playerRotation);
        var networkGunObject = gunObject.GetComponent<NetworkObject>();
        networkGunObject.Spawn();

        // update the gun list on player's gunHolder
        switchWeapon.UpdateGunList();
    }
}
