using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private Camera cam;
    private PlayerSwitchWeapon switchWeapon;

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
        switchWeapon = GetComponentInChildren<PlayerSwitchWeapon>();
        promptText = string.Empty;
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
                promptText = interactable.promptMessage;
            }
            
        }
    }

    
    public void DropHoldingGun ()
    {
        
        int currentGunIndex = switchWeapon.selectedWeapon;
        if (switchWeapon.guns[currentGunIndex] != null)
        {
            if (switchWeapon.guns[currentGunIndex].CanShoot()) //switchWeapon.guns[currentGunIndex].IsOwned()
            {
                //GameObject gunToDrop = switchWeapon.guns[currentGunIndex].gunCounterpart.gameObject;
                Vector3 spawnPosition = transform.position + transform.forward * 2;
                Vector3 aimDir = (cam.transform.forward).normalized;
                Quaternion gunRotation = Quaternion.LookRotation(aimDir, Vector3.up);
                DropHoldingGuntServerRpc(spawnPosition, gunRotation);
                switchWeapon.guns[currentGunIndex].gameObject.SetActive(false);
                switchWeapon.guns[currentGunIndex].UpdateIsOwned(false);
            }
        }
    }

    
    [ServerRpc]
    private void DropHoldingGuntServerRpc(Vector3 playerPosition, Quaternion playerRotation)
    {

        int currentGunIndex = switchWeapon.selectedWeapon;
        GameObject gunToDrop = switchWeapon.guns[currentGunIndex].gunCounterpart.gameObject;
        var gunObject = Instantiate(gunToDrop.gameObject, playerPosition, playerRotation);
        var networkGunObject = gunObject.GetComponent<NetworkObject>();
        networkGunObject.Spawn();
    }
    

}
