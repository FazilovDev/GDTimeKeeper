using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace
{
    public class Manager
    {
        Animator _WeaponSpaceAnimator;

        int _SelectedWeaponIndex;
        WeaponRuntimeHandler[] _WeaponRuntimeHandlers;

        Transform _ProjectileSpawn;
        Transform _BarrelFlashSpawn;
        Transform _EjectedCartridgeSpawn;

        AudioSource _BarrelFlashAudioSource;

        GameObject _InstantiatedWeaponObject;

        CharacterController CharacterControllerProperty { get; set; }
        Systems.WeaponCollection WeaponCollectionProperty { get; set; }
        Transform CameraRaySpawn { get; set; }
        Transform WeaponSpace { get; set; }
        
        Animator WeaponSpaceAnimator
        {
            get => _WeaponSpaceAnimator;
            set => _WeaponSpaceAnimator = value;
        }

        public int SelectedWeaponIndex
        {
            get => _SelectedWeaponIndex;
            private set => _SelectedWeaponIndex = value;
        }
        public WeaponRuntimeHandler[] WeaponRuntimeHandlers
        {
            get => _WeaponRuntimeHandlers;
            set => _WeaponRuntimeHandlers = value;
        }
        List<Systems.Weapon> Weapons { get; set; }

        Transform ProjectileSpawn
        {
            get => _ProjectileSpawn;
            set => _ProjectileSpawn = value;
        }
        Transform BarrelFlashSpawn
        {
            get => _BarrelFlashSpawn;
            set => _BarrelFlashSpawn = value;
        }
        Transform EjectedCartridgeSpawn
        {
            get => _EjectedCartridgeSpawn;
            set => _EjectedCartridgeSpawn = value;
        }

        AudioSource BarrelFlashAudioSource
        {
            get => _BarrelFlashAudioSource;
            set => _BarrelFlashAudioSource = value;
        }

        GameObject InstantiatedWeaponObject
        {
            get => _InstantiatedWeaponObject;
            set => _InstantiatedWeaponObject = value;
        }

        Fire.Manager FireManager { get; set; }
        Reload.Manager ReloadManager { get; set; }
        Switch.Manager SwitchManager { get; set; }
        Aim.Manager AimManager { get; set; }
        Input.Manager InputManager { get; set; }
        UI.Manager UIManager { get; set; }
        Movement.Manager MovementManager { get; set; }
        public Enable.Manager EnableManager { get; set; }   // # temp public
        public Ammo.Manager AmmoManager { get; set; }      // # temp public

        public Manager(
            CharacterController characterController,
            Systems.WeaponCollection weaponCollection,
            Animator weaponSpaceAnimator,
            AudioSource weaponSpaceAudioSource,
            MonoBehaviour scriptMonoBehaviour,
            Transform weaponSpaceTransform,
            Transform cameraRaySpawn,
            KeyCode fireInput,
            KeyCode autoFireInput,
            KeyCode reloadInput,
            KeyCode switchInput,
            KeyCode aimInput,
            KeyCode runInput,
            Image crosshairSpace,
            Image ammoIconSpace,
            Text weaponAmmoCountSpace,
            Text totalAmmoCountSpace,
            GameObject bloodSplatterImpact,
            string[] raycastIgnorableLayerNames
            )
        {
            CharacterControllerProperty = characterController;
            WeaponCollectionProperty = weaponCollection;
            CameraRaySpawn = cameraRaySpawn;
            WeaponSpace = weaponSpaceTransform;
            WeaponSpaceAnimator = weaponSpaceAnimator;

            Weapons = WeaponCollectionProperty.weapons;

            WeaponRuntimeHandlers = new WeaponRuntimeHandler[Weapons.Count];
            WeaponRuntimeHandler.InitialiseArray(ref _WeaponRuntimeHandlers);

            AmmoManager = new Ammo.Manager(Weapons, WeaponRuntimeHandlers);

            for (int i = 0; i < Weapons.Count; i++)
            {
                if (
                        !AmmoManager.TotalAmmoCounts.ContainsKey(WeaponCollectionProperty.weapons[i].ammo.name)
                    )
                {
                    AmmoManager.TotalAmmoCounts.Add(
                        WeaponCollectionProperty.weapons[i].ammo.name,
                        WeaponCollectionProperty.weapons[i].ammo.startAmount
                        );
                }
            }

            InputManager = new Input.Manager(
                fireInput,
                autoFireInput,
                reloadInput,
                switchInput,
                aimInput,
                runInput
                );

            FireManager = new Fire.Manager(
                WeaponSpaceAnimator,
                InputManager,
                bloodSplatterImpact,
                raycastIgnorableLayerNames
                );

            ReloadManager = new Reload.Manager(
                InputManager,
                scriptMonoBehaviour,
                WeaponSpaceAnimator,
                weaponSpaceAudioSource
                );

            Weapons = weaponCollection.weapons;

            EnableManager = new Enable.Manager(
                Weapons,
                WeaponRuntimeHandlers
                );

            SwitchManager = new Switch.Manager(
                WeaponSpaceAnimator,
                weaponSpaceAudioSource,
                Weapons.Count,
                InputManager
                );

            AimManager = new Aim.Manager(
                WeaponSpaceAnimator, 
                InputManager
                );

            MovementManager = new Movement.Manager(
                characterController,
                WeaponSpaceAnimator,
                InputManager
                );

            UIManager = new UI.Manager(
                crosshairSpace,
                ammoIconSpace,
                weaponAmmoCountSpace,
                totalAmmoCountSpace,
                AmmoManager.TotalAmmoCounts
                );
        }

        public void Start(Transform weaponSpaceTransform)
        {
            // Removes all child object of FPSWeaponSystems (that were used in edit mode)
            for (int i = 0; i < weaponSpaceTransform.childCount; i++)
            {
                Object.Destroy(weaponSpaceTransform.GetChild(i).gameObject);
            }

            EnableManager.EnableWeaponsOnStart();

            AmmoManager.ApplyWeaponsAmmoCountOnStart(
                );

            // These variables are used because Properties cannot be applied in ref parameters 
            int refBurstShotCount = FireManager.BurstShotCount;

            SwitchManager.Start(
                WeaponCollectionProperty.weapons,
                _WeaponRuntimeHandlers,
                ref refBurstShotCount,
                weaponSpaceTransform,
                ref _InstantiatedWeaponObject,
                ref _BarrelFlashSpawn,
                ref _ProjectileSpawn,
                ref _EjectedCartridgeSpawn,
                ref _BarrelFlashAudioSource,
                ref _WeaponSpaceAnimator,
                ref _SelectedWeaponIndex
                );

            FireManager.BurstShotCount = refBurstShotCount;
        }

        public void Update(Transform weaponSpaceTransform, string mouseXInfluenceName, string mouseYInfluenceName)
        {
            Systems.Weapon weapon = WeaponCollectionProperty.weapons[SelectedWeaponIndex];

            UpdateInput();

            UpdateFire(weapon);
            UpdateReload(weapon);
            UpdateSwitch(weaponSpaceTransform);

            UpdateAim(weapon);
            UpdateMovement(weapon, mouseXInfluenceName, mouseYInfluenceName);

            UpdateUI(weapon);

            UpdateEnable();
            UpdateAmmo();
        }

        void UpdateFire(Systems.Weapon weapon)
        {
            bool[] ConflictingStates = {
                ReloadManager.IsReloading,
                SwitchManager.IsSwitching,
                MovementManager.CharacterBasedInfluencerProperty.IsRunning
            };
            float[] ConflictingCountDowns = {
                MovementManager.CharacterBasedInfluencerProperty.RunningRecoveryCountDown,
                ReloadManager.IncrementalReloadInterruptionCountDown
            };

            int refBurstShotCount = FireManager.BurstShotCount;

            FireManager.Update(
                weapon,
                SelectedWeaponIndex,
                ref _WeaponRuntimeHandlers,
                ref refBurstShotCount,
                ConflictingStates, 
                ConflictingCountDowns,
                AimManager.CurrentAimingInterpolation,
                MovementManager.CharacterBasedInfluencerProperty.IsWalking,
                CameraRaySpawn,
                BarrelFlashSpawn,
                ProjectileSpawn,
                EjectedCartridgeSpawn,
                BarrelFlashAudioSource,
                WeaponSpace.forward,
                CharacterControllerProperty.velocity
                );

            FireManager.BurstShotCount = refBurstShotCount;
        }

        void UpdateReload(Systems.Weapon weapon)
        {
            Dictionary<string, int> totalAmmoCounts = AmmoManager.TotalAmmoCounts;

            bool[] conflictingStates = { 
                SwitchManager.IsSwitching,
                MovementManager.CharacterBasedInfluencerProperty.IsRunning 
            };

            int refBurstShotCount = FireManager.BurstShotCount;

            ReloadManager.Update(
                weapon,
                ref _WeaponRuntimeHandlers[SelectedWeaponIndex].WeaponAmmoCount,
                ref refBurstShotCount,
                InputManager.IsReloadDetected,
                ref totalAmmoCounts,
                conflictingStates
                );

            FireManager.BurstShotCount = refBurstShotCount;

            AmmoManager.TotalAmmoCounts = totalAmmoCounts;
        }

        void UpdateSwitch(Transform transform)
        {
            int refBurstShotCount = FireManager.BurstShotCount;

            SwitchManager.Update(
               WeaponCollectionProperty.weapons,
               _WeaponRuntimeHandlers,
               ref refBurstShotCount,
               transform,
               ref _SelectedWeaponIndex,
               ref _InstantiatedWeaponObject,
               ref _BarrelFlashSpawn,
               ref _ProjectileSpawn,
               ref _EjectedCartridgeSpawn,
               ref _BarrelFlashAudioSource,
               ref _WeaponSpaceAnimator
               );

            FireManager.BurstShotCount = refBurstShotCount;
        }

        void UpdateAim(Systems.Weapon weapon)
        {
            bool[] isFalseArray = { 
                ReloadManager.IsReloading,
                SwitchManager.IsSwitching, 
                MovementManager.CharacterBasedInfluencerProperty.IsRunning 
            };
            
            AimManager.Update(
                weapon,
                isFalseArray
                );
        }

        void UpdateInput()
        {
            InputManager.Update();
        }

        void UpdateUI(Systems.Weapon weapon)
        {
            UIManager.Update(
                SwitchManager.IsSwitching,
                weapon,
                _WeaponRuntimeHandlers[SelectedWeaponIndex].WeaponAmmoCount,
                AimManager.CurrentAimingInterpolation
                );
        }

             // ? any need for these parameters?
        void UpdateMovement(Systems.Weapon weapon, string mouseXInfluenceName, string mouseYInfluenceName)
        {
            MovementManager.Update(
                weapon,
                // Input.GetAxis() is used here as sensitity does not work outside MonoBehaviour scripts   
                UnityEngine.Input.GetAxis(mouseXInfluenceName),
                UnityEngine.Input.GetAxis(mouseYInfluenceName)
                );
        }

        void UpdateEnable()
        {
            EnableManager.Update(ref _WeaponRuntimeHandlers);
        }

        void UpdateAmmo()
        {
            AmmoManager.Update(ref _WeaponRuntimeHandlers);
        }
    }
}