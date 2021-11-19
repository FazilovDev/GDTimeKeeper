using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Switch
{
    public class Manager
    {
        bool _IsSwitching = false;
        bool _IsCurrentWeaponDisabled = false;
        Systems.Weapon _CurrentWeapon;

        public bool IsSwitching
        {
            get => _IsSwitching;
            set => _IsSwitching = value;
        }
        bool IsCurrentWeaponDisabled
        {
            get => _IsCurrentWeaponDisabled;
            set => _IsCurrentWeaponDisabled = value;
        }
        Systems.Weapon CurrentWeapon 
        {
            get => _CurrentWeapon;
            set => _CurrentWeapon = value;
        }
        Systems.Weapon NextWeapon { get; set; }

        Selector SelectorProperty { get; set; }
        Switcher SwitcherProperty { get; set; }
        Loader LoaderProperty { get; set; }
        Input.Manager InputManager { get; set; }

        public Manager(
            Animator animator,
            AudioSource audioSource,
            int weaponCount,
            Input.Manager inputManager
            )
        {
            SelectorProperty = new Selector(weaponCount);
            SwitcherProperty = new Switcher(animator, audioSource);
            LoaderProperty = new Loader();
            InputManager = inputManager;
        }

        public void Start(
            List<Systems.Weapon> weapons,
            WeaponRuntimeHandler[] weaponRuntimeHandlers,
            ref int burstShotCount,
            Transform weaponSpaceTransform,
            ref GameObject weaponInstance,
            ref Transform barrelFlashSpawn,
            ref Transform projectileSpawn,
            ref Transform ejectedCartridgeSpawn,
            ref AudioSource barrelFlashAudioSource,
            ref Animator weaponAnimator,
            ref int currentWeaponIndex
            )
        {
            currentWeaponIndex = SelectorProperty.StartWeaponIndex(weaponRuntimeHandlers);

            CurrentWeapon = weapons[SelectorProperty.StartWeaponIndex(weaponRuntimeHandlers)];

            LoaderProperty.LoadWeapon(
                CurrentWeapon, 
                ref burstShotCount,
                weaponSpaceTransform,
                ref weaponAnimator,
                ref weaponInstance,
                ref barrelFlashSpawn,
                ref projectileSpawn,
                ref ejectedCartridgeSpawn,
                ref barrelFlashAudioSource
                );
        }

        public void Update(
            List<Systems.Weapon> weapons,
            WeaponRuntimeHandler[] weaponRuntimeHandlers,
            ref int burstShotCount,
            Transform weaponSpaceTransform,
            ref int selectedWeaponIndex,
            ref GameObject weaponInstance,
            ref Transform barrelFlashSpawn,
            ref Transform projectileSpawn,
            ref Transform ejectedCartridgeSpawn,
            ref AudioSource barrelFlashAudioSource,
            ref Animator weaponAnimator
            )
        {
            if (!weaponRuntimeHandlers[selectedWeaponIndex].IsWeaponEnabled)
            {
                IsCurrentWeaponDisabled = true;
            }

            if (
                    (
                        (
                            InputManager.IsSwitchDetected 
                            &&
                            IsAnotherWeaponsEnabled(WeaponRuntimeHandler.GetIsWeaponEnabledArray(weaponRuntimeHandlers))
                        )
                        || 
                        IsCurrentWeaponDisabled
                    )
                    && 
                    !IsSwitching
                )
            {
                NextWeapon = weapons[SelectorProperty.NextWeaponIndex(weaponRuntimeHandlers)];
                IsSwitching = true;
            }

            SwitcherProperty.Update(
                ref _CurrentWeapon, 
                NextWeapon, 
                ref _IsSwitching, 
                ref _IsCurrentWeaponDisabled, 
                ref burstShotCount,
                weaponSpaceTransform,
                ref weaponInstance,
                ref barrelFlashSpawn,
                ref projectileSpawn,
                ref ejectedCartridgeSpawn,
                ref barrelFlashAudioSource,
                ref weaponAnimator
                );

            selectedWeaponIndex = General.ArrayConverter.ElementToIndex(CurrentWeapon, weapons);
        }

        public static bool IsAnotherWeaponsEnabled(bool[] isWeaponEnabledArray)
        {
            int weaponsEnabledCount = General.ArrayChecker.NumberOfTrueStates(isWeaponEnabledArray);

            if (weaponsEnabledCount > 1)
            {
                return true;
            }
            else if (weaponsEnabledCount <= 0)
            {
                // ? add instructive text
                General.ErrorHandling.ErrorMessenger.ThrowException(
                    "At least one weapon must be enabled."
                    );
            }

            return false;
        }

    }
}