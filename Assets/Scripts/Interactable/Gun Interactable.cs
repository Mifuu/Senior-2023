using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunInteractable : InteractableItem
{
    public Gun gunCounterpart; // this one is spawned to player's had when player pick up (interact) with this interactable gun
    private GameObject playerObject;

    protected override void Interact(ulong PlayerId)
    {
        Debug.Log("interacted with" + gameObject.name);
        playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(PlayerId).gameObject;
        if (playerObject != null)
        {
            if (gunCounterpart != null)
            {
                /*
                Gun[] playerGuns = playerObject.GetComponentsInChildren<Gun>(true);
                foreach (Gun playerGun in playerGuns)
                {
                    if (playerGun.name == gunCounterpart.name)
                    {
                        Debug.Log("Found and matched Gun counterpart: " + playerGun.name);
                        playerGun.gameObject.SetActive(true);
                        SelfDespawnServerRpc();
                        NetworkObject.Despawn(true);
                        break;
                    }
                }
                */
                SpawnGunCounterpartServerRpc();
            }
            else
            {
                Debug.LogError("Cannot find Gun gunCounterpart: please check if you have assigned the variable to the script");
            }
        }
        else
        {
            Debug.LogError("Game Object " + gameObject.name + "cannot find interacted player's GameObject");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelfDespawnServerRpc()
    {
        NetworkObject.Despawn(true);
    }

    
    [ServerRpc]
    public void SpawnGunCounterpartServerRpc()
    {
        PlayerSwitchWeapon gunHolder = playerObject.GetComponentInChildren<PlayerSwitchWeapon>();
        if (gunHolder == null)
        {
            Debug.LogError("Gun interactable Script: gunHolder is null");
            return;
        }
        if (gunCounterpart == null)
        {
            Debug.LogError("Gun interactable Script: gunCounterpart is null");
            return;
        }
        if (gunHolder.IsFull())
        {
            
            int currentIndex = gunHolder.selectedWeapon.Value;
            var gunObject = Instantiate(gunCounterpart.gameObject, gunHolder.transform.position, gunHolder.transform.rotation);
            var gunNetworkObj = gunObject.GetComponent<NetworkObject>();
            gunNetworkObj.Spawn();
            PlayerInteract playerInteract = playerObject.GetComponent<PlayerInteract>();
            playerInteract.DropHoldingGun();
            gunNetworkObj.transform.SetParent(gunHolder.transform);
            gunNetworkObj.transform.SetSiblingIndex(currentIndex);
        }
        else
        {
            // spawn the counterpart of the gun infront of the player
            var gunObject = Instantiate(gunCounterpart.gameObject, gunHolder.transform.position, gunHolder.transform.rotation);
            var gunNetworkObj = gunObject.GetComponent<NetworkObject>();
            gunNetworkObj.Spawn();
            gunNetworkObj.transform.SetParent(gunHolder.transform);
            gunHolder.UpdateGunList();
        }
        Debug.Log($"Gun interactable try to spawn {gunCounterpart.name}");

    }

    [ServerRpc]
    public void InteractServerRpc()
    {

    }
}
