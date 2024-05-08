using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunHolderSpawner : NetworkBehaviour
{
    //[SerializeField] public PlayerSwitchWeapon gunHolder;
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject gunHolder;
   
    void Start()
    {
        if (!IsOwner)
        if (gunHolder == null)
        {
            Debug.LogError("GunHolderSpawner: gunholder is null");
            return;
        }
        //gunHolder = Resources.Load<GameObject>("GunHolder");
        SpawnGunHolderServerRpc();
    }

    [ServerRpc]
    private void SpawnGunHolderServerRpc()
    {
        var gunHolderObj = Instantiate(gunHolder, transform.position, transform.rotation);
        var gunHolderNetworkObj = gunHolderObj.GetComponent<NetworkObject>();
        gunHolderNetworkObj.Spawn();
        gunHolderNetworkObj.TrySetParent(player.transform);
    }
}
