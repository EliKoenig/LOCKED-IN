using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public GameObject[] weapons;
    int currentWeaponIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        weapons = new GameObject[2];
        weapons[0] = GameObject.FindWithTag("Primary");  // Make sure Weapon1 has the "Weapon1" tag
        weapons[1] = GameObject.FindWithTag("Sidearm");  // Make sure Weapon2 has the "Weapon2" tag

        // Disable all weapons except the first one at the start
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);  // Switch to weapon 1

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);  // Switch to weapon 2
        }

    }

        void SwitchWeapon(int weaponIndex)
    {
        // Only switch if the selected weapon is not already active
        if (weaponIndex != currentWeaponIndex)
        {
            weapons[currentWeaponIndex].SetActive(false);  // Disable the current weapon
            weapons[weaponIndex].SetActive(true);  // Enable the new weapon
            currentWeaponIndex = weaponIndex;  // Update the current weapon index
        }
    }
}
