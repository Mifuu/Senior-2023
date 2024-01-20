using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSwitchWeapon : NetworkBehaviour
{
    public int selectedWeapon = 0;
    private Gun[] guns; 

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
                if (i == selectedWeapon)
                {
                    guns[i].gameObject.SetActive(true);
                }
                else
                {
                    guns[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SwitchWeapon(float index)
    {
        if (IsClient && IsOwner)
        {
            if (index == 1)
            {
                if (guns[selectedWeapon].CanShoot())
                {
                    Debug.Log("switch to 0");
                    selectedWeapon = 0;
                    SelectWeapon();

                    // Update canShoot for the current gun
                    guns[selectedWeapon].UpdateCanShoot(true);
                }
            }

            if (index == 2)
            {
                if (guns[selectedWeapon].CanShoot())
                {
                    Debug.Log("switch to 1");
                    selectedWeapon = 1;
                    SelectWeapon();

                    // Update canShoot for the current gun
                    guns[selectedWeapon].UpdateCanShoot(true);
                }
            }
        }
    }
}
