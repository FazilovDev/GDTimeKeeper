using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Movement
{
    public class CharacterBasedInfluencer
    {
        public bool IsJumping { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;
        public bool IsWalking { get; private set; } = false;

        public float RunningRecoveryCountDown { get; private set; } = 0f;

        CharacterController CharacterControllerProperty { get; set; }
        Animator AnimatorProperty { get; set; }

        public CharacterBasedInfluencer(
            CharacterController characterController,
            Animator animator
            )
        {
            CharacterControllerProperty = characterController;
            AnimatorProperty = animator;
        }

        public void Update(
            Systems.Weapon weapon,
            bool IsRuningInputDetected
            )
        {
            // Decrement running transition
            if (RunningRecoveryCountDown > 0f)
            {
                RunningRecoveryCountDown -= 1f * Time.deltaTime;
            }
            else
            {
                RunningRecoveryCountDown = 0f;
            }

            IsJumping = false;
            IsRunning = false;
            IsWalking = false;

            if (!CharacterControllerProperty.isGrounded)
            {
                IsJumping = true;
            }
            else if (new Vector2(CharacterControllerProperty.velocity.x, CharacterControllerProperty.velocity.z).magnitude > 0f)
            {
                if (IsRuningInputDetected)
                {
                    IsRunning = true;
                }
                else
                {
                    IsWalking = true;
                }
            }

            // Animation effected by the states
            AnimatorProperty.SetBool(weapon.jumpAnimationVarName, false);
            AnimatorProperty.SetBool(weapon.runAnimationVarName, false);
            AnimatorProperty.SetBool(weapon.walkAnimationVarName, false);

            if (IsJumping)
            {
                AnimatorProperty.SetBool(weapon.jumpAnimationVarName, true);
            }

            if (IsRunning)
            {
                AnimatorProperty.SetBool(weapon.runAnimationVarName, true);
                RunningRecoveryCountDown = weapon.runningRecoveryTime;
            }

            if (IsWalking)
            {
                AnimatorProperty.SetBool(weapon.walkAnimationVarName, true);
            }
        }
    }
}