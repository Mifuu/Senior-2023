using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System.Linq;

public class PlayerSwitchWeapon : NetworkBehaviour
{
    public NetworkVariable<int> currentGunIndex = new NetworkVariable<int> (0);
    public Gun[] guns = new Gun[3];
    public int maxGun = 3;
    public NetworkObject initialGun_1;
    public NetworkObject initialGun_2;
    public NetworkObject initialGun_3;
    private GameObject player;
    private PlayerShoot playerShoot;
    private InputManager inputManager;
    private PlayerInteract playerInteract;
    private NetworkObject gunToSpawn;

    void Start()
    {
        player = transform.parent.gameObject;
        playerShoot = player.GetComponent<PlayerShoot>();
        inputManager = player.GetComponent<InputManager>();
        playerInteract = player.GetComponent<PlayerInteract>();
        playerShoot.InitializePlayerSwitchWeapon();
        inputManager.InitializePlayerSwitchWeapon();
        playerInteract.InitializePlayerSwitchWeapon();

        gunToSpawn = initialGun_1;
        SpawnGunServerRpc(0);
        gunToSpawn = initialGun_2;
        SpawnGunServerRpc(1);
        gunToSpawn = initialGun_3;
        SpawnGunServerRpc(2);
        UpdateGunList();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentGunIndex.OnValueChanged += UpdateWeapon;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        currentGunIndex.OnValueChanged -= UpdateWeapon;
    }

    [ServerRpc]
    private void SpawnGunServerRpc(int index)
    {
        Debug.Log($"try to spawn gun {gunToSpawn.name}");
        if (gunToSpawn == null)
        {
            Debug.LogError("PlayerSwitchWeapon Script: gunToSpawn is null");
            return;
        }

        if (index < guns.Length)
        {
            var gunNetworkObj = Instantiate(gunToSpawn, transform.position, transform.rotation);
            gunNetworkObj.Spawn();
            gunNetworkObj.transform.SetParent(transform); // Set the player as the parent of the gun
            guns[index] = gunNetworkObj.GetComponent<Gun>();
        }
    }

    public void UpdateGunList()
    {
        Gun[] _guns = new Gun[3];
        _guns = GetComponentsInChildren<Gun>(includeInactive: true);
        guns = _guns;
        AdjustCurrentGunIndex();
        SelectWeapon();
    }
    public void UpdateWeapon(int previous, int current)
    {
        AdjustCurrentGunIndex();
        SelectWeapon();
        guns[currentGunIndex.Value].UpdateCanShoot(true);
    }

    private void AdjustCurrentGunIndex()
    {
        if (currentGunIndex.Value == guns.Length)
        {
            currentGunIndex.Value = (guns.Length - 1);
            if (currentGunIndex.Value < 0)
            {
                currentGunIndex.Value = 0;
            }
        }
    }
    public bool IsFull()
    {
        if (guns.Length == maxGun)
        {
            return true;
        }
        return false;
    }

    void SelectWeapon()
    {
        if (!IsOwner) return;
        
        for (int i = 0; i < guns.Length; i++)
        {
            if( i == currentGunIndex.Value )
            {
                guns[i].gameObject.SetActive(true);
            }
            else
            {
                guns[i].gameObject.SetActive(false);
            }
        }
    }

    public void SwitchWeapon(float index)
    {
        if (!IsOwner) return;
        
        Debug.Log("Player Script: Switch activate");
        AdjustCurrentGunIndex();
        int currentWeaponIndex = currentGunIndex.Value;
        int newWeaponIndex = Mathf.FloorToInt(index - 1);
        if (newWeaponIndex >= 0 && newWeaponIndex < guns.Length && guns[currentWeaponIndex].CanShoot())
        {
            Debug.Log($"Player Script: Switch to weapon {newWeaponIndex + 1}");
            currentGunIndex.Value = newWeaponIndex;
            //ChangeSelectedWeaponServerRPC(newWeaponIndex);
        }
    }

    public GameObject GetHoldingGun()
    {
        if (guns[currentGunIndex.Value].gameObject == null)
        {
            Debug.LogError("PlayerSwitchWeapon Script: GameObject of current gun is null");
            return null;
        }
        GameObject currentGun = guns[currentGunIndex.Value].gameObject;
        return currentGun;
    }

    /*
    [ServerRpc]
    public void ChangeSelectedWeaponServerRPC(int index)
    {
        currentGunIndex.Value = index;
    }
    */
}
