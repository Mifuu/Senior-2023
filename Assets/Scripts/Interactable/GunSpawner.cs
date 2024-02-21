using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunSpawner : MonoBehaviour
{
    [SerializeField] public GameObject gunPrefab;

    public void SpawnGun()
    {
        // Check if we are the server before spawning the gun
        if (NetworkManager.Singleton.IsServer)
        {
            // Spawn the gun prefab
            NetworkObject gun = Instantiate(gunPrefab, transform.position, transform.rotation).GetComponent<NetworkObject>();
            gun.Spawn();
        }
    }
}
