using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.Netcode;

public class PlayerSwitchWeapon : NetworkBehaviour
{
    public int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the selected weapon to active and the others to not active
    void SelectWeapon()
    {
        if (IsClient && IsOwner)
        {
            int i = 0;
            foreach (Transform weapon in transform)
            {
                if (i == selectedWeapon)
                {
                    weapon.gameObject.SetActive(true);
                }
                else
                {
                    weapon.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }

    public void SwitchWeapon1()
    {
        selectedWeapon = 1;
    }

    public void SwitchWeapon(float index)
    {
        if (index == 1)
        {
            Debug.Log("switch to 0");
            selectedWeapon = 0;
            SelectWeapon();
        }

        if (index == 2)
        {
            Debug.Log("switch to 1");
            selectedWeapon = 1;
            SelectWeapon();
        }
    }
}
