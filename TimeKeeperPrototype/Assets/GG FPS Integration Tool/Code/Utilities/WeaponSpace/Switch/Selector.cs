using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Switch
{
    public class Selector
    {
        public int CurrentIndex { get; private set; } = 0;
        int WeaponCount { get; set; }

        public Selector(int weaponCount)
        {
            WeaponCount = weaponCount;
        }

        public int StartWeaponIndex(
            WeaponRuntimeHandler[] weaponRuntimeHandlers
            )
        {
            // Only the error throwing (for when no weapons are enabled on start) is needed
            // Hence bool return is not used
            Manager.IsAnotherWeaponsEnabled(WeaponRuntimeHandler.GetIsWeaponEnabledArray(weaponRuntimeHandlers));

            while (!weaponRuntimeHandlers[CurrentIndex].IsWeaponEnabled)
            {
                CurrentIndex++;

                if (CurrentIndex >= WeaponCount)
                {
                    CurrentIndex = 0;
                }
            }

            return CurrentIndex;
        }

        public int NextWeaponIndex(
            WeaponRuntimeHandler[] weaponRuntimeHandlers
            )
        {
            if (
                    !
                    Manager.IsAnotherWeaponsEnabled(WeaponRuntimeHandler.GetIsWeaponEnabledArray(weaponRuntimeHandlers))
                    &&
                    // Prevents mulfunction when switching to the only remaining weapon to select
                    // after the currently equipt weapon is disabled
                    weaponRuntimeHandlers[CurrentIndex].IsWeaponEnabled
                )
            {
                return CurrentIndex;
            }

            do
            {
                CurrentIndex++;

                if (CurrentIndex >= WeaponCount)
                {
                    CurrentIndex = 0;
                }
            }
            while (!weaponRuntimeHandlers[CurrentIndex].IsWeaponEnabled);

            return CurrentIndex;
        }
    }
}