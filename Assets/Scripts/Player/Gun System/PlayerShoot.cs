using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private PlayerSwitchWeapon weaponSwitching;

    private Camera playerCam;
    public LayerMask aimColliderLayerMask;

    void Start()
    {
        playerCam = GetComponent<PlayerLook>().cam;   
    }

    public void InitializePlayerSwitchWeapon()
    {
        weaponSwitching = transform.GetComponentInChildren<PlayerSwitchWeapon>();
    }

    public void ShootBullet()
    {
        if (!IsOwner) return;
        if (weaponSwitching == null)
        {
            Debug.LogError("Player Shoot: weaponSwitching is null");
            return;
        }

        int selectedWeaponIndex = weaponSwitching.currentGunIndex.Value;
        GameObject selectedWeaponObject = weaponSwitching.transform.GetChild(selectedWeaponIndex).gameObject;
        Gun selectedGun = selectedWeaponObject.GetComponent<Gun>();
        if (selectedGun != null && selectedGun.CanShoot())
        {
            selectedGun.ShootBullet_(playerCam, aimColliderLayerMask);
        }
        
    }
}
