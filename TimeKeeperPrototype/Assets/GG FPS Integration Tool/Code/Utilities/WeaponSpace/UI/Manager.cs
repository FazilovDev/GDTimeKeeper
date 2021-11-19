using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.UI
{
    public class Manager
    {
        Image CrosshairSpace { get; set; }
        Image AmmoIconSpace { get; set; }
        Text WeaponAmmoCountText { get; set; }
        Text TotalAmmoCountText { get; set; }
        Dictionary<string, int> Ammos { get; set; }

        public Manager(
            Image crosshairSpace,
            Image ammoIconSpace,
            Text weaponAmmoCountText,
            Text totalAmmoCountText,
            Dictionary<string, int> ammos
            )
        {
            CrosshairSpace = crosshairSpace;
            AmmoIconSpace = ammoIconSpace;
            WeaponAmmoCountText = weaponAmmoCountText;
            TotalAmmoCountText = totalAmmoCountText;
            Ammos = ammos;
        }

        public void Update(
            bool isSwitching,
            Systems.Weapon weapon,
            int ammoInWeapon,
            float currentAimingTime
            )
        {
            if (isSwitching) return;
            
            CrosshairSpace.sprite = weapon.crosshairSprite;

            if (currentAimingTime <= 0f)
            {
                CrosshairSpace.color = weapon.crosshairColour;
            }
            else
            {
                Color c = weapon.crosshairColour;
                CrosshairSpace.color = new Color(c.r, c.g, c.b, 0f);
            }

            AmmoIconSpace.sprite = weapon.ammo.ammoIcon;

            WeaponAmmoCountText.text = ammoInWeapon.ToString();
            TotalAmmoCountText.text = Ammos[weapon.ammo.name].ToString();
        }
    }
}