using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Reload
{
    public class Manager
    {
        
        bool _IsReloading = false;
        float _IncrementalReloadInterruptionCountDown;

        public bool IsReloading
        {
            get => _IsReloading;
            private set => _IsReloading = value;
        }
        public float IncrementalReloadInterruptionCountDown
        {
            get => _IncrementalReloadInterruptionCountDown;
            private set => _IncrementalReloadInterruptionCountDown = value;
        }

        public Starter StarterProperty { get; private set; }
        public Stopper StopperProperty { get; private set; }

        public Manager(
            Input.Manager inputManager,
            MonoBehaviour monoBehaviour,
            Animator animator,
            AudioSource audioSource
            )
        {
            Coroutine.Manager coroutineManager = new Coroutine.Manager(
                monoBehaviour,
                animator,
                audioSource
                );

            StarterProperty = new Starter(coroutineManager);
            StopperProperty = new Stopper(coroutineManager, inputManager, animator, audioSource);
        }

        public void Update(
            Systems.Weapon weapon,
            ref int weaponAmmoCount,
            ref int burstShotCount,
            bool isReloadInputDetected,
            ref Dictionary<string, int> totalAmmoCounts,
            bool[] isConflictingStateTrueArray
            )
        {
            General.Time.CountDown.ManualDeltaTimeDecrement(ref _IncrementalReloadInterruptionCountDown);

            StopperProperty.Update(
                weapon,
                weaponAmmoCount,
                ref burstShotCount,
                ref _IncrementalReloadInterruptionCountDown,
                ref _IsReloading,
                isConflictingStateTrueArray
                );

            StarterProperty.Update(
                weapon,
                isReloadInputDetected,
                weaponAmmoCount,
                totalAmmoCounts[weapon.ammo.name],
                IsReloading,
                isConflictingStateTrueArray
                );

            // As indexers are not allowed in ref parameters
            int refTotalAmmoCount = totalAmmoCounts[weapon.ammo.name];

            StarterProperty.CoroutineManager.UpdateCallback(
                ref _IsReloading,
                ref weaponAmmoCount,
                ref refTotalAmmoCount,
                ref burstShotCount,
                ref _IncrementalReloadInterruptionCountDown
                );

            totalAmmoCounts[weapon.ammo.name] = refTotalAmmoCount;
        }
    }
}
