using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Ammo
{
    public class Manager
    {
        public Dictionary<string, int> TotalAmmoCounts { get; set; }

        List<Systems.Weapon> Weapons { get; set; }
        WeaponRuntimeHandler[] WeaponRuntimeHandlers { get; set; }
        bool CanCallbackBeRead { get; set; }

        public Manager(
            List<Systems.Weapon> weapons,
            WeaponRuntimeHandler[] weaponRuntimeHandlers
            )
        {
            TotalAmmoCounts = new Dictionary<string, int>();

            Weapons = weapons;
            WeaponRuntimeHandlers = weaponRuntimeHandlers;
        }

        public void ApplyWeaponsAmmoCountOnStart()
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (
                        // enableOnStart is included as loadedAtStart can be hidden in the Inspector
                        // when enableOnStart is false
                        Weapons[i].enableOnStart 
                        && 
                        Weapons[i].loadedAtStart
                    )
                {
                    WeaponRuntimeHandlers[i].WeaponAmmoCount = Weapons[i].capacity;
                }
            }
        }

        public void Update(
            ref WeaponRuntimeHandler[] weaponRuntimeHandlers
            )
        {
            if (CanCallbackBeRead)
            {
                weaponRuntimeHandlers = WeaponRuntimeHandlers;

                CanCallbackBeRead = false;
            }
            else
            {
                WeaponRuntimeHandlers = weaponRuntimeHandlers;
            }
        }

        public void WeaponAmmoCount(
            Systems.Weapon weapon,
            int ammoCount,
            Systems.Ammo weaponAmmo
            )
        {
            ammoCount = Mathf.Clamp(
                ammoCount,
                0, 
                weapon.capacity
                );

            // If weapon has already been enabled when collected,
            // apply weapon's ammo to total ammo count
            if (WeaponRuntimeHandlers[Weapons.IndexOf(weapon)].IsWeaponEnabled)
            {
                IncreaseTotalAmmoCount(weaponAmmo, ammoCount);
            }
            else
            {
                WeaponRuntimeHandlers[Weapons.IndexOf(weapon)].WeaponAmmoCount = ammoCount;
            }

            CanCallbackBeRead = true;
        }

        public void IncreaseTotalAmmoCount(
            Systems.Ammo targetAmmo,
            int amountToAdd
            )
        {
            TotalAmmoCounts[targetAmmo.name] += amountToAdd;

            if (TotalAmmoCounts[targetAmmo.name] < 0)
            {
                TotalAmmoCounts[targetAmmo.name] = 0;
            }
        }
    }
}