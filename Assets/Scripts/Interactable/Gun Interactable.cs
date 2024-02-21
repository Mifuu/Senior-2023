using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunInteractable : InteractableItem
{
    public Gun gunCounterpart;

    protected override void Interact(ulong PlayerId)
    {
        Debug.Log("interacted with" + gameObject.name);
        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(PlayerId).gameObject;
        if (playerObject != null)
        {
            if (gunCounterpart != null)
            {
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
    public void InteractServerRpc()
    {

    }
}
