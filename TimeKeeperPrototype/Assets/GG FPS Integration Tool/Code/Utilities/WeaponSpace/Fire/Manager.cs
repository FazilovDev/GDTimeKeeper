using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Fire
{
    public class Manager
    {
        public int BurstShotCount { get; set; }

        public Authoriser AuthoriserProperty { get; private set; }
        public FireHandler FireManagerProperty { get; private set; }
        public Visual.Manager VisualManager { get; private set; }
        public Input.Manager InputManager { get; private set; }

        
        public Manager(
            Animator animator,
            Input.Manager inputManager,
            GameObject bloodSplatterImpact,
            string[] raycastIgnorableLayerNames
            )
        {
            InputManager = inputManager;

            AuthoriserProperty = new Authoriser();

            FireManagerProperty = new FireHandler(
                bloodSplatterImpact,
                raycastIgnorableLayerNames
                );

            VisualManager = new Visual.Manager(animator);
        }

        public void Update(
            Systems.Weapon weapon,
            int selectedWeaponIndex,
            ref WeaponRuntimeHandler[] weaponRuntimeHandlers,
            ref int burstShotCount,
            bool[] isConflictingStateFalseArray,
            float[] conflictingCountDowns,
            float aimingInterpolationValue,
            bool isPlayerMoving,
            Transform raySpawn,
            Transform barrelFlashSpawn,
            Transform projectileSpawn,
            Transform cartridgeSpawn,
            AudioSource barrelFlashAudioSource,
            Vector3 forwardVector,
            Vector3 playerVelocity
            )
        {
            NextShotCountDownHandler.Update(ref weaponRuntimeHandlers);

            VisualManager.Update(weapon.fireAnimationVarName);

            if (
                !
                AuthoriserProperty.IsFireAuthorised(
                    (int)weapon.firingType,
                    InputManager.IsFireDetected,
                    InputManager.IsAutoFireDetected,
                    burstShotCount,
                    weapon.roundsPerBurst,
                    weaponRuntimeHandlers[selectedWeaponIndex].NextShotCountDown,
                    weaponRuntimeHandlers[selectedWeaponIndex].WeaponAmmoCount,
                    weapon.ammoLossPerRound,
                    isConflictingStateFalseArray,
                    conflictingCountDowns
                    )
                )
            {
                return;
            }

            if (weapon.outputType == 0)
            {
                FireManagerProperty.FireWeapon(
                    weapon,
                    aimingInterpolationValue,
                    isPlayerMoving,
                    raySpawn
                    );
            }
            else
            {
                FireManagerProperty.FireWeapon(
                    weapon,
                    aimingInterpolationValue,
                    isPlayerMoving,
                    projectileSpawn,
                    forwardVector
                    );
            }

            barrelFlashAudioSource.Play();

            VisualManager.CreateVisuals(
                weapon.fireAnimationVarName,
                weapon.barrelFlash,
                barrelFlashSpawn,
                weapon.ejectedCartridge.ejectedObject,
                cartridgeSpawn,
                weapon.ejectedCartridge.ejectionTrajectory,
                weapon.ejectedCartridge.ejectionForce,
                playerVelocity
                );

            ManageBurstShots(
                ref burstShotCount, 
                weapon.roundsPerBurst
                );
            AfterCalculation(
                ref weaponRuntimeHandlers[selectedWeaponIndex].NextShotCountDown, 
                weapon.fireRate, 
                ref weaponRuntimeHandlers[selectedWeaponIndex].WeaponAmmoCount, 
                weapon.ammoLossPerRound
                );
        }

        void ManageBurstShots(
            ref int burstShotCount, 
            int shotsPerBurst
            )
        {
            burstShotCount--;

            if (burstShotCount <= 0)
            {
                burstShotCount = shotsPerBurst;
            }
        }

        void AfterCalculation(
            ref float nextShotCountDown, 
            float weaponFireRate, 
            ref int weaponAmmoCount, 
            int ammoLossPerShot
            )
        {
            nextShotCountDown = 1f / weaponFireRate;
            weaponAmmoCount -= ammoLossPerShot;
        }
    }
}