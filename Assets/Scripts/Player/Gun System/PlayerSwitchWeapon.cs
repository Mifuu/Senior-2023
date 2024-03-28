using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;


public class PlayerSwitchWeapon : NetworkBehaviour
{
    public NetworkVariable<int> selectedWeapon = new NetworkVariable<int> (0);
    public Gun[] guns = new Gun[3];
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
        //SelectWeapon();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        selectedWeapon.OnValueChanged += UpdateWeapon;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        selectedWeapon.OnValueChanged -= UpdateWeapon;
    }

    /*
    void SpawnGun(int index, NetworkObject gunToSpawn)
    {
        Debug.Log($"try to spawn gun {gunToSpawn.name}");
        if (gunToSpawn == null)
        {
            Debug.LogError("PlayerSwitchWeapon Script: gunToSpawn is null");
            return;
        }

        if (index <  guns.Length)
        {
            GameObject gunObject = Instantiate(gunToSpawn, transform.position, transform.rotation);
            gunObject.transform.SetParent(transform); // Set the player as the parent of the gun
            guns[index] = gunObject.GetComponent<Gun>();
        }
    }
    */

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
        SelectWeapon();
    }
    public void UpdateWeapon(int previous, int current)
    {
        SelectWeapon();
        guns[selectedWeapon.Value].UpdateCanShoot(true);
    }

    void SelectWeapon()
    {
        if (!IsOwner) return;
        
        for (int i = 0; i < guns.Length; i++)
        {
            if( i == selectedWeapon.Value )
            {
                guns[i].gameObject.SetActive(true);
            }
            else
            {
                guns[i].gameObject.SetActive(false);
            }
            //guns[i].gameObject.SetActive(i == selectedWeapon.Value);
        }
    }

    public void SwitchWeapon(float index)
    {
        if (IsClient && IsOwner)
        {
            Debug.Log("Player Script: Switch activate");
            int currentWeaponIndex = selectedWeapon.Value;
            int newWeaponIndex = Mathf.FloorToInt(index - 1);
            if (newWeaponIndex >= 0 && newWeaponIndex < guns.Length && guns[currentWeaponIndex].CanShoot())
            {
                /*
                if (guns[currentWeaponIndex].IsOwned())
                {
                    Debug.Log($"Player Script: Switch to weapon {newWeaponIndex + 1}");
                    selectedWeapon = newWeaponIndex;
                    SelectWeapon();
                    guns[selectedWeapon].UpdateCanShoot(true);
                }
                */

                Debug.Log($"Player Script: Switch to weapon {newWeaponIndex + 1}");
                ChangeSelectedWeaponServerRPC(newWeaponIndex);
            }
        }
    }

    [ServerRpc]
    public void ChangeSelectedWeaponServerRPC(int index)
    {
        selectedWeapon.Value = index;
    }
}
