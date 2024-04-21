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
        gunHolder = Resources.Load<GameObject>("GunHolder");
        SpawnGunHolderServerRpc();
    }

    [ServerRpc]
    private void SpawnGunHolderServerRpc()
    {
        if (!IsOwner) return;
        if (gunHolder == null)
        {
            Debug.LogError("GunHolderSpawner: gunholder is null");
        }
        var gunHolderObj = Instantiate(gunHolder, transform.position, transform.rotation);
        var gunHolderNetworkObj = gunHolderObj.GetComponent<NetworkObject>();
        gunHolderNetworkObj.Spawn();
        gunHolderNetworkObj.transform.SetParent(player.transform);
    }
}
