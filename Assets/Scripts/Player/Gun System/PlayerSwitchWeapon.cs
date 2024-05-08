using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using UnityEngine.Events;
using System.Linq;
using Unity.Netcode;
/* using Microsoft.Unity.VisualStudio.Editor; */

public class PlayerSwitchWeapon : NetworkBehaviour
{
    public Subject<int> currentGunIndex = new(0);
    public Gun[] guns = new Gun[3];
    public int maxGun = 3;
    private GameObject player;
    private PlayerShoot playerShoot;
    private InputManager inputManager;
    private PlayerInteract playerInteract;
    private PlayerUIUpdater playerUIUpdater;
    private BuffManager buffManager;
    //private Playergunenabler playerGunEnabler;

    public Subject<float> gunShootingSpeedMultiplier = new(1f);

    void Start()
    {
        player = transform.parent.gameObject;
        playerShoot = player.GetComponent<PlayerShoot>();
        inputManager = player.GetComponent<InputManager>();
        playerInteract = player.GetComponent<PlayerInteract>();
        playerUIUpdater = player.GetComponentInChildren<PlayerUIUpdater>();
        buffManager = player.GetComponent<BuffManager>();
       // playerGunEnabler = player.GetComponent<Playergunenabler>();
        playerShoot.InitializePlayerSwitchWeapon();
        inputManager.InitializePlayerSwitchWeapon();
        playerInteract.InitializePlayerSwitchWeapon();
        playerUIUpdater.InitializePlayerSwitchWeapon();
        buffManager.InitializePlayerSwitchWeapon();
        UpdateGunList();

        currentGunIndex.OnValueChanged += UpdateWeapon;
        gunShootingSpeedMultiplier.OnValueChanged += (prev, current) => SetHoldingGunShootingSpeedMultiplier();
    }

    public void UpdateGunList()
    {
        Gun[] _guns = new Gun[3];
        _guns = GetComponentsInChildren<Gun>(includeInactive: true);
        guns = _guns;
        AdjustCurrentGunIndex();
        SelectWeapon();
        playerUIUpdater.UpdateGunSprite(guns.Length > 0 ? guns[0].gunSprite : null, guns.Length > 1 ? guns[1].gunSprite : null, guns.Length > 2 ? guns[2].gunSprite : null);
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
        //if (!IsOwner) return;
        
        for (int i = 0; i < guns.Length; i++)
        {
            if( i == currentGunIndex.Value )
            {
                guns[i].gameObject.SetActive(true);
                SetHoldingGunShootingSpeedMultiplier(); // set the multiplier of the holding gun
                Debug.Log("PlayerSwitchWeapon: gunShootingSpeedMultiplier is " + gunShootingSpeedMultiplier);
                playerUIUpdater.UpdateSelectedGunSlotColor(i);
            }
            else
            {
                guns[i].gameObject.SetActive(false);
            }
        }
    }

    public void SwitchWeapon(float index)
    {
        //if (!IsOwner) return;
        
        Debug.Log("Player Script: Switch activate");
        AdjustCurrentGunIndex();
        int currentWeaponIndex = currentGunIndex.Value;
        int newWeaponIndex = Mathf.FloorToInt(index - 1);
        if (newWeaponIndex >= 0 && newWeaponIndex < guns.Length && guns[currentWeaponIndex].CanShoot())
        {
            Debug.Log($"Player Script: Switch to weapon {newWeaponIndex + 1}");
            currentGunIndex.Value = newWeaponIndex;
            //ChangeSelectedWeaponServerRPC(newWeaponIndex);
            playerUIUpdater.UpdateGunSprite(guns.Length > 0 ? guns[0].gunSprite : null, guns.Length > 1 ? guns[1].gunSprite : null, guns.Length > 2 ? guns[2].gunSprite : null);

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

    private void SetHoldingGunShootingSpeedMultiplier ()
    {
        guns[currentGunIndex.Value].shootingSpeedMultiplier = gunShootingSpeedMultiplier.Value;
    }
}
