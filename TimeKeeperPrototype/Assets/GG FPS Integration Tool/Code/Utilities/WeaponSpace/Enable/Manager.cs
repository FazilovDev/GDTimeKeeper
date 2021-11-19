using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Enable
{
    public class Manager
    {
        List<Systems.Weapon> Weapons { get; set; }
        WeaponRuntimeHandler[] WeaponRuntimeHandlers { get; set; }
        bool CanCallbackBeRead { get; set; }

        public Manager(
            List<Systems.Weapon> weapons,
            WeaponRuntimeHandler[] weaponRuntimeHandlers
            )
        {
            Weapons = weapons;
            WeaponRuntimeHandlers = weaponRuntimeHandlers;
        }

        public void EnableWeaponsOnStart()
        {
            // ? should check if both array sizes match first?
            for (int i = 0; i < WeaponRuntimeHandlers.Length; i++)
            {
                WeaponRuntimeHandlers[i].IsWeaponEnabled = Weapons[i].enableOnStart;
            }

            CanCallbackBeRead = true;
        }

        // Used to update properties within this class and used for callbacks
        // Allows function calls (EnableWeapon etc) to omit parameters 
        // which would require private properties
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

        public void EnableWeapon(
            int targetWeaponIndex
            )
        {
            WeaponRuntimeHandlers[targetWeaponIndex].IsWeaponEnabled = true;

            CanCallbackBeRead = true;
        }

            // ? mention in documentation that Weapon SOs cannot have the same name
            // or weapon collections cant have more than one of the same weapon
        public void EnableWeapon(
            Systems.Weapon targetWeapon
            )
        {
            int matchedWeaponIndex = Weapons.IndexOf(targetWeapon);

            if (matchedWeaponIndex == -1)
            {
                General.ErrorHandling.ErrorMessenger.ThrowException(
                    "The Weapon named '" + targetWeapon.name + "' cannot be found in WeaponCollection.",
                    "Ensure that all weapon that will be used in scene are assigned in the WeaponCollection."
                    );
            }

            WeaponRuntimeHandlers[matchedWeaponIndex].IsWeaponEnabled = true;

            CanCallbackBeRead = true;
        }

        public void DisableWeapon(
            int targetWeaponIndex
            )
        {
            WeaponRuntimeHandlers[targetWeaponIndex].IsWeaponEnabled = false;

            CanCallbackBeRead = true;
        }

        public void DisableWeapon(
            Systems.Weapon targetWeapon
            )
        {
            int matchedWeaponIndex = Weapons.IndexOf(targetWeapon);

            if (matchedWeaponIndex == -1)
            {
                General.ErrorHandling.ErrorMessenger.ThrowException(
                    "The Weapon named '" + targetWeapon.name + "' cannot be found in WeaponCollection.",
                    "Ensure that all weapon that will be used in scene are assigned in the WeaponCollection."
                    );
            }

            WeaponRuntimeHandlers[matchedWeaponIndex].IsWeaponEnabled = false;

            CanCallbackBeRead = true;
        }
    }
}