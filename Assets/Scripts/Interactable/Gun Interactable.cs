using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GunInteractable : InteractableItem
{
    public Gun gunCounterpart; // this one is spawned to player's had when player pick up (interact) with this interactable gun
    private GameObject playerObject;
    private ElementalType gunElement;

    private void Start()
    {
        gunElement = GetComponent<ElementAttachable>().element;
    }

    protected override void Interact(GameObject playerObject)
    {
        Debug.Log("interacted with" + gameObject.name);
        if (playerObject != null)
        {
            this.playerObject = playerObject;
            if (gunCounterpart != null)
            {
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
        // check null
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

        // spawn gun to player's gunHolder
        if (gunHolder.IsFull())
        {
            Debug.Log("gunHolder is full, player drop current gun to pick up");
            int currentIndex = gunHolder.currentGunIndex.Value;
            var gunObject = Instantiate(gunCounterpart.gameObject, gunHolder.transform.position, gunHolder.transform.rotation);
            gunObject.GetComponent<ElementAttachable>().element = gunElement; 
            var gunNetworkObj = gunObject.GetComponent<NetworkObject>();
            gunNetworkObj.Spawn();
            PlayerInteract playerInteract = playerObject.GetComponent<PlayerInteract>();
            playerInteract.DropHoldingGun();
            gunNetworkObj.transform.SetParent(gunHolder.transform);
            gunNetworkObj.transform.SetSiblingIndex(currentIndex);
        }
        else
        {
            Debug.Log("gunHolder is not full, player pick up a gun and place it in the last slot");
            // spawn the counterpart of the gun infront of the player
            var gunObject = Instantiate(gunCounterpart.gameObject, gunHolder.transform.position, gunHolder.transform.rotation);
            gunObject.GetComponent<ElementAttachable>().element = gunElement;
            var gunNetworkObj = gunObject.GetComponent<NetworkObject>();
            gunNetworkObj.Spawn();
            gunNetworkObj.transform.SetParent(gunHolder.transform);
            gunHolder.UpdateGunList();
        }
        SelfDespawnServerRpc();
        Debug.Log($"Gun interactable try to spawn {gunCounterpart.name}");
    }

    [ServerRpc]
    public void InteractServerRpc()
    {

    }
}
