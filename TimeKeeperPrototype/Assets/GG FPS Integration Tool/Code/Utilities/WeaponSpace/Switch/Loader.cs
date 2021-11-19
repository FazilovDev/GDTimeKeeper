using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EH = GGFPSIntegrationTool.Utilities.General.ErrorHandling;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Switch
{
    public class Loader
    {
        // Number of characters to remove from the end of instance's name, usually being "(clone)" hence being 7 by default
        // Prevents animations from disconnecting with object
        public byte CharactersToShortenInstanceNameBy { get; set; } = 7;

        public void LoadWeapon(
            Systems.Weapon weapon, 
            ref int burstShotCount,
            Transform weaponSpaceTransform,
            ref Animator weaponSpaceAnimator,
            ref GameObject weaponInstance,
            ref Transform barrelFlashSpawn,
            ref Transform projectileSpawn,
            ref Transform ejectedCartridgeSpawn,
            ref AudioSource barrelFlashAudioSource
            )
        {
            DestroyLastWeapon(ref weaponInstance);
            InstantiateWeapon(weapon, weaponSpaceTransform, ref weaponInstance);
            FindWeaponSpawnPoints(weapon, ref barrelFlashSpawn, ref projectileSpawn, ref ejectedCartridgeSpawn);

            barrelFlashAudioSource = barrelFlashSpawn.GetComponent<AudioSource>();
            barrelFlashAudioSource.clip = weapon.barrelSound;
            
            burstShotCount = weapon.roundsPerBurst;

            // Prevents animation disconnection issue (if first weapon controller is pre-applied to animator)
            weaponSpaceAnimator.runtimeAnimatorController = null;
            weaponSpaceAnimator.runtimeAnimatorController = weapon.animatorController;
        }

        void DestroyLastWeapon(ref GameObject weaponInstance)
        {
            if (weaponInstance != null)
            {
                Object.Destroy(weaponInstance);
            }
        }

        void InstantiateWeapon(
            Systems.Weapon weapon,
            Transform weaponSpaceTransform,
            ref GameObject weaponInstance
            )
        {
            // Spawn new weapon instance
            if (weapon.weaponPrefab != null)
            {
                weaponInstance = Object.Instantiate(weapon.weaponPrefab, weaponSpaceTransform);

                string name = weaponInstance.name;
                weaponInstance.name = name.Remove(name.Length - CharactersToShortenInstanceNameBy);
            }
        }

        void FindWeaponSpawnPoints(
            Systems.Weapon weapon,
            ref Transform barrelFlashSpawn,
            ref Transform projectileSpawn,
            ref Transform ejectedCartridgeSpawn
            )
        {
            barrelFlashSpawn = GameObject.Find(weapon.barrelFlashSpawnName).transform;
            projectileSpawn = GameObject.Find(weapon.projectileSpawnName).transform;
            ejectedCartridgeSpawn = GameObject.Find(weapon.cartridgeSpawnName).transform;

            const string BarrelFlashSpawnErrorMessage = "Ensure Barrel Flash Spawn Name field matches GameObject's name.";
            const string ProjectileSpawnErrorMessage = "Ensure Projectile Spawn Name field matches GameObject's name.";
            const string EjectedCartridgeSpawnErrorMessage = "Ensure Ejected Cartridge Spawn Name field matches GameObject's name.";

            EH.NullChecker.ThrowIfNull(barrelFlashSpawn, nameof(barrelFlashSpawn), BarrelFlashSpawnErrorMessage);
            EH.NullChecker.ThrowIfNull(projectileSpawn, nameof(projectileSpawn), ProjectileSpawnErrorMessage);
            EH.NullChecker.ThrowIfNull(ejectedCartridgeSpawn, nameof(ejectedCartridgeSpawn), EjectedCartridgeSpawnErrorMessage);

        }
    }
}
