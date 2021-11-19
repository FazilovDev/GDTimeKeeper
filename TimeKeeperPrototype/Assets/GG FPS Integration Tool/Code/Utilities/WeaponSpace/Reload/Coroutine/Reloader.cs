using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Reload.Coroutine
{
    public class Reloader
    {
        public int FullReloadingTypeIndex { get; set; } = 0;

        public void ReloadWeapon(
            ref int weaponAmmoCount,
            ref int totalAmmoCount,
            int reloadingTypeIndex,
            int weaponCapacity,
            int ammoAddedPerReload
            )
        {
            // Will reload fill weapon capacity completely
            if (
                reloadingTypeIndex == FullReloadingTypeIndex
                ||
                ammoAddedPerReload > (weaponCapacity - weaponAmmoCount)
                )
            {
                // Is there enough ammo for reload
                if (totalAmmoCount < (weaponCapacity - weaponAmmoCount))
                {
                    // Reload weapon with remaining total ammo
                    weaponAmmoCount += totalAmmoCount;
                    totalAmmoCount = 0;
                }
                else
                {
                    // Reload weapon fully
                    totalAmmoCount -= weaponCapacity - weaponAmmoCount;
                    weaponAmmoCount = weaponCapacity;
                }
            }
            else
            {
                // Is there enough ammo for reload
                if (totalAmmoCount < ammoAddedPerReload)
                {
                    // Reload weapon with remaining total ammo
                    weaponAmmoCount += totalAmmoCount;
                    totalAmmoCount = 0;
                }
                else
                {
                    // Reload weapon with expected partial amount
                    totalAmmoCount -= ammoAddedPerReload;
                    weaponAmmoCount += ammoAddedPerReload;
                }
            }
        }
    }
}