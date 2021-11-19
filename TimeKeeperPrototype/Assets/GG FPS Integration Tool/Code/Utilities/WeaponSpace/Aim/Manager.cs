using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Aim
{
    public class Manager
    {
        public float CurrentAimingInterpolation { get; private set; } = 0;

        Animator AnimatorProperty { get; set; }
        Input.Manager InputManager { get; set; }

        public Manager(
            Animator animator,
            Input.Manager inputManager
            )
        {
            AnimatorProperty = animator;
            InputManager = inputManager;
        }

        public void Update(
            Systems.Weapon weapon,
            bool[] isConflictingStateFalseArray
            )
        {
            if (
                    InputManager.IsAimDetected 
                    && 
                    General.ArrayChecker.AreAllFalse(isConflictingStateFalseArray)
                )
            {
                AnimatorProperty.SetBool(weapon.aimAnimationVarName, true);
                UpdateAimingInterpolation(true, weapon.aimingTime);
            }
            else
            {
                AnimatorProperty.SetBool(weapon.aimAnimationVarName, false);
                UpdateAimingInterpolation(false, weapon.aimingTime);
            }
        }

        void UpdateAimingInterpolation(
            bool isAiming,
            float weaponAimingTime
            )
        {
            float interpolationChange = (1f / weaponAimingTime) * Time.deltaTime;

            if (isAiming)
            {
                CurrentAimingInterpolation += interpolationChange;
            }
            else
            {
                CurrentAimingInterpolation -= interpolationChange;
            }

            CurrentAimingInterpolation = Mathf.Clamp01(CurrentAimingInterpolation);
        }
    }
}