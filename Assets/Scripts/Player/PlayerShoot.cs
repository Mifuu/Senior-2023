using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerShoot : NetworkBehaviour
{

    [SerializeField] private PlayerSwitchWeapon weaponSwitching;
    
    private Camera playerCam;
    private LayerMask aimColliderLayerMask;
    private Transform debugTransform;

    void Start()
    {
        // Assign necessary references (camera, layer mask, debug transform)
        playerCam = GetComponent<PlayerLook>().cam;
    }

    void Update()
    {
       
    }

    public void ShootBullet()
    {
        if (IsClient && IsOwner)
        {
            Debug.Log("weapon: " + weaponSwitching.selectedWeapon);
            int selectedWeaponIndex = weaponSwitching.selectedWeapon;
            GameObject selectedWeaponObject = weaponSwitching.transform.GetChild(selectedWeaponIndex).gameObject;
            Gun selectedGun = selectedWeaponObject.GetComponent<Gun>();
            if (selectedGun != null)
            {
                selectedGun.ShootBullet(playerCam, aimColliderLayerMask, debugTransform);
            }
            else
            {
                Debug.LogWarning("Selected weapon does not have a Gun component");
            }
        }
    }
}
