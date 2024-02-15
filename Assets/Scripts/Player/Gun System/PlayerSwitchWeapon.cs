using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSwitchWeapon : NetworkBehaviour
{
    public int selectedWeapon = 0;
    public Gun[] guns; 

    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        SelectWeapon();
    }

    void SelectWeapon()
    {
        if (IsClient && IsOwner)
        {
            for (int i = 0; i < guns.Length; i++)
            {
                guns[i].gameObject.SetActive(i == selectedWeapon);
            }
        }
    }

    public void SwitchWeapon(float index)
    {
        if (IsClient && IsOwner)
        {
            Debug.Log("Player Script: Switch activate");
            int currentWeaponIndex = selectedWeapon;
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
                selectedWeapon = newWeaponIndex;
                SelectWeapon();
                guns[selectedWeapon].UpdateCanShoot(true);
            }
        }
    }
}
