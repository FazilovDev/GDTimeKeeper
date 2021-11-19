using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Reload
{
    public class Stopper
    {
        public int SemiautoFiringTypeIndex { get; set; } = 0;
        public int AutoFiringTypeIndex { get; set; } = 1;
        public int PartialRepeatReloadingTypeIndex { get; set; } = 2;

        public Coroutine.Manager CoroutineManager { get; private set; }
        public Input.Manager InputManager { get; private set; }
        public Animator AnimatorProperty { get; private set; }
        public AudioSource AudioSourceProperty { get; private set; }

        public Stopper(Coroutine.Manager coroutineManager, Input.Manager inputManager, Animator animator, AudioSource audioSource)
        {
            CoroutineManager = coroutineManager;
            InputManager = inputManager;
            AnimatorProperty = animator;
            AudioSourceProperty = audioSource;
        }

        public void Update(
            Systems.Weapon weapon,
            int weaponAmmoCount,
            ref int burstShotCount,
            ref float incrementalReloadInterruptionCountDown,
            ref bool isReloading,
            bool[] isConflictingStateFalseArray
            )
        {
            if (
                    isReloading
                    &&
                    (
                        !General.ArrayChecker.AreAllFalse(isConflictingStateFalseArray)
                        ||
                        (
                            weaponAmmoCount > 0
                            &&
                            (
                                ((int)weapon.firingType == SemiautoFiringTypeIndex && InputManager.IsFireDetected)
                                ||
                                ((int)weapon.firingType == AutoFiringTypeIndex && InputManager.IsAutoFireDetected)
                            )
                        )
                    )
                )
            {
                CoroutineManager.StopCoroutine();

                AudioSourceProperty.Stop();
                AnimatorProperty.SetBool(weapon.reloadAniamtionVarName, false);

                burstShotCount = weapon.roundsPerBurst;
                incrementalReloadInterruptionCountDown = weapon.incrementalReloadRecoveryTime;
            }
        }
    }
}