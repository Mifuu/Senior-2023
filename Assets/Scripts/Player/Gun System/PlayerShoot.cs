using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private PlayerSwitchWeapon weaponSwitching;

    private Camera playerCam;
    public LayerMask aimColliderLayerMask;
    private Transform debugTransform;

    void Start()
    {
        playerCam = GetComponent<PlayerLook>().cam;
    }

    public void ShootBullet()
    {
        if (IsClient && IsOwner)
        {
            int selectedWeaponIndex = weaponSwitching.selectedWeapon;
            GameObject selectedWeaponObject = weaponSwitching.transform.GetChild(selectedWeaponIndex).gameObject;
            Gun selectedGun = selectedWeaponObject.GetComponent<Gun>();
            if (selectedGun != null && selectedGun.CanShoot())
            {
                selectedGun.ShootBullet(playerCam, aimColliderLayerMask, debugTransform);
            }
        }
    }
}
