using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Reload.Coroutine
{
    public class Manager
    {
        // Member variables are used as Properties cannot be applied in ref parameters
        int _WeaponAmmoCount;
        int _TotalAmmoCount;

        public int PartialReloadingTypeIndex { get; set; } = 1;
        public int PartialRepeatReloadingTypeIndex { get; set; } = 2;

        public MonoBehaviour MonoBehaviourProperty { get; private set; }
        public Animator AnimatorProperty { get; private set; }
        public AudioSource AudioSourceProperty { get; private set; }
        public Reloader ReloaderProperty { get; private set; } = new Reloader();

        public bool IsReloading { get; private set; } = false;
        bool CanCallbackBeRead { get; set; } = false;

        UnityEngine.Coroutine CoroutineProperty { get; set; }
        int BurstShotCount { get; set; }
        float IncrementalReloadInterruptionCountDown { get; set; } // Applies to Partial & PartialRepeat
        int WeaponAmmoCount
        {
            get => _WeaponAmmoCount;
            set => _WeaponAmmoCount = value;
        }
        int TotalAmmoCount
        {
            get => _TotalAmmoCount;
            set => _TotalAmmoCount = value;
        }

        public Manager(MonoBehaviour monoBehaviour, Animator animator, AudioSource audioSource)
        {
            MonoBehaviourProperty = monoBehaviour;
            AnimatorProperty = animator;
            AudioSourceProperty = audioSource;
        }

        public void StartCoroutine(
            Systems.Weapon weapon,
            int weaponAmmoCount,
            int totalAmmoCount
            )
        {
            // Checks if reloading type is PartialRepeat, such reloading has different coroutine
            if ((int)weapon.reloadingType != PartialRepeatReloadingTypeIndex)
            {
                CoroutineProperty = MonoBehaviourProperty.StartCoroutine(
                    ReloadWeaponOnce(
                        weaponAmmoCount,
                        totalAmmoCount,
                        (int)weapon.reloadingType,
                        weapon.reloadingTime,
                        weapon.capacity,
                        weapon.roundsPerBurst,
                        weapon.ammoAddedPerReload,
                        weapon.reloadSound,
                        weapon.reloadAniamtionVarName,
                        weapon.incrementalReloadRecoveryTime
                        )
                    );
            }
            else
            {
                CoroutineProperty = MonoBehaviourProperty.StartCoroutine(
                    ReloadWeaponRepeatedly(
                        weaponAmmoCount,
                        totalAmmoCount,
                        (int)weapon.reloadingType,
                        weapon.reloadingTime,
                        weapon.capacity,
                        weapon.roundsPerBurst,
                        weapon.ammoAddedPerReload,
                        weapon.reloadSound,
                        weapon.reloadAniamtionVarName,
                        weapon.incrementalReloadRecoveryTime
                        )
                    );
            }
        }

        // Aborts the reloading process
        public void StopCoroutine()
        {
            if (CoroutineProperty != null)
            {
                MonoBehaviourProperty.StopCoroutine(CoroutineProperty);
            }

            StopCallback();
        }

        // Ensures values used in coroutine are only accessible during execution
        // Prevents values (such as ammo) being assigned to the wrong weapon
        public void UpdateCallback(
            ref bool isReloading,
            ref int weaponAmmoCount,
            ref int totalAmmoCount,
            ref int burstShotCount,
            ref float incrementalReloadInterruptionCountDown
            )
        {
            isReloading = IsReloading;

            // Ensures coroutine values can only be read once after reloading has ended
            if (CanCallbackBeRead)
            {
                weaponAmmoCount = WeaponAmmoCount;
                totalAmmoCount = TotalAmmoCount;
                burstShotCount = BurstShotCount;
                incrementalReloadInterruptionCountDown = IncrementalReloadInterruptionCountDown;

                CanCallbackBeRead = false;
            }

            if (IsReloading)
            {
                CanCallbackBeRead = true;
            }
        }

        // Completely stops UpdateCallback from updating ref parameters
        // Prevents values being applied to the wrong weapon when switching weapon while reloading
        void StopCallback()
        {
            IsReloading = false;
            CanCallbackBeRead = false;
        }

        IEnumerator ReloadWeaponOnce(
            int weaponAmmoCount,
            int totalAmmoCount,
            int reloadingTypeIndex,
            float reloadingTime,
            int weaponCapacity,
            int shotsPerBurst,
            int ammoAddedPerReload,
            AudioClip reloadAudio,
            string reloadAnimationParameterName,
            float incrementalReloadInterruptionCountDown
            )
        {
            IsReloading = true;

            _WeaponAmmoCount = weaponAmmoCount;
            _TotalAmmoCount = totalAmmoCount;

            AudioSourceProperty.clip = reloadAudio;
            AudioSourceProperty.Play();
            AnimatorProperty.SetBool(reloadAnimationParameterName, true);

            yield return new WaitForSeconds(reloadingTime);

            AnimatorProperty.SetBool(reloadAnimationParameterName, false);

            ReloaderProperty.ReloadWeapon(
                ref _WeaponAmmoCount, 
                ref _TotalAmmoCount, 
                reloadingTypeIndex, 
                weaponCapacity, 
                ammoAddedPerReload
                );

            BurstShotCount = shotsPerBurst;

            // Prevents firing when weapon is transitioning from reloading to idle animations
            if (reloadingTypeIndex == PartialReloadingTypeIndex)
            {
                IncrementalReloadInterruptionCountDown = incrementalReloadInterruptionCountDown;
            }

            IsReloading = false;
        }

        IEnumerator ReloadWeaponRepeatedly(
            int weaponAmmoCount,
            int totalAmmoCount,
            int reloadingTypeIndex,
            float reloadingTime,
            int weaponCapacity,
            int shotsPerBurst,
            int ammoAddedPerReload,
            AudioClip reloadAudio,
            string reloadAnimationParameterName,
            float incrementalReloadInterruptionCountDown
            )
        {
            IsReloading = true;

            _WeaponAmmoCount = weaponAmmoCount;
            _TotalAmmoCount = totalAmmoCount;

            AudioSourceProperty.clip = reloadAudio;

            AnimatorProperty.SetBool(reloadAnimationParameterName, true);

            // Repeat reloading process
            for (int i = WeaponAmmoCount; i < weaponCapacity; i += ammoAddedPerReload)
            {
                AudioSourceProperty.Play();

                yield return new WaitForSeconds(reloadingTime);

                ReloaderProperty.ReloadWeapon(ref _WeaponAmmoCount, ref _TotalAmmoCount, reloadingTypeIndex, weaponCapacity, ammoAddedPerReload);

                // Ensures reloading stops when total ammo runs out
                if (TotalAmmoCount <= 0)
                {
                    break;
                }
            }

            AnimatorProperty.SetBool(reloadAnimationParameterName, false);

            BurstShotCount = shotsPerBurst;

            // Prevents firing when weapon is transitioning from reloading to idle animations
            IncrementalReloadInterruptionCountDown = incrementalReloadInterruptionCountDown;

            IsReloading = false;
        }
    }
}