using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Reload
{
    public class Starter
    {
        public Coroutine.Manager CoroutineManager { get; private set; }

        public Starter(Coroutine.Manager coroutineManager)
        {
            CoroutineManager = coroutineManager;
        }

        public void Update(
            Systems.Weapon weapon,
            bool isReloadInputDetected,
            int weaponAmmoCount,
            int totalAmmoCount,
            bool isReloading,
            bool[] isConflictingStateTrueArray
            )
        {
            if (
                    (
                        weaponAmmoCount < weapon.ammoLossPerRound
                        ||
                        (weaponAmmoCount < weapon.capacity && isReloadInputDetected)
                    )
                    &&
                    totalAmmoCount > 0
                    &&
                    !isReloading
                    &&
                    General.ArrayChecker.AreAllFalse(isConflictingStateTrueArray)
                )
            {
                CoroutineManager.StartCoroutine(
                    weapon,
                    weaponAmmoCount,
                    totalAmmoCount
                    );
            }
        }
    }
}