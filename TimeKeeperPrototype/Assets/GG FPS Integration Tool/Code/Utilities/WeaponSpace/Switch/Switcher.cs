using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Switch
{
    public class Switcher
    {
        Animator AnimatorProperty { get; set; }
        AudioSource AudioSourceProperty { get; set; }
        Loader LoaderProperty { get; set; }
        General.Time.CountDown SwitchOutCountDown { get; set; }
        General.Time.CountDown SwitchInCountDown { get; set; }

        public Switcher(Animator animator, AudioSource audioSource)
        {
            AnimatorProperty = animator;
            AudioSourceProperty = audioSource;

            LoaderProperty = new Loader();
            SwitchOutCountDown = new General.Time.CountDown();
            SwitchInCountDown = new General.Time.CountDown();
        }

        public void Update(
            ref Systems.Weapon currentWeapon, 
            Systems.Weapon nextWeapon,
            ref bool isSwitching,
            ref bool isCurrentWeaponDisabled,
            ref int burstShotCount,
            Transform weaponSpaceTransform,
            ref GameObject weaponInstance,
            ref Transform barrelFlashSpawn,
            ref Transform projectileSpawn,
            ref Transform ejectedCartridgeSpawn,
            ref AudioSource barrelFlashAudioSource,
            ref Animator weaponAnimator
            )
        {
            if (!isSwitching) return;

            SwitchOutCountDown.Update();
            SwitchInCountDown.Update();

            if (!SwitchOutCountDown.HasStarted && !SwitchInCountDown.HasStarted)
            {
                AudioSourceProperty.clip = currentWeapon.switchOutSound;
                AudioSourceProperty.Play();
                AnimatorProperty.SetBool(currentWeapon.switchAnimationVarName, true);

                SwitchOutCountDown.Start(currentWeapon.switchingTime);
            }
            else if (SwitchOutCountDown.HasEnded && !SwitchInCountDown.HasStarted)
            {
                LoaderProperty.LoadWeapon(
                    nextWeapon, 
                    ref burstShotCount,
                    weaponSpaceTransform,
                    ref weaponAnimator,
                    ref weaponInstance,
                    ref barrelFlashSpawn,
                    ref projectileSpawn,
                    ref ejectedCartridgeSpawn,
                    ref barrelFlashAudioSource
                    );

                AudioSourceProperty.clip = nextWeapon.switchInSound;
                AudioSourceProperty.Play();

                AnimatorProperty.SetBool(nextWeapon.switchAnimationVarName, false);

                SwitchInCountDown.Start(nextWeapon.switchingTime);

            }
            else if (SwitchInCountDown.HasEnded)
            {
                isSwitching = false;
                isCurrentWeaponDisabled = false;

                currentWeapon = nextWeapon;
            }
        }
    }
}
