using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Fire
{
    public static class NextShotCountDownHandler
    {
        public static void Update(ref WeaponRuntimeHandler[] weaponRuntimeHandlers)
        {
            float[] nextShotCountDowns = WeaponRuntimeHandler.GetNextShotCountDowns(weaponRuntimeHandlers);

            General.Time.CountDown.ManualDeltaTimeDecrement(ref nextShotCountDowns);

            WeaponRuntimeHandler.SetNextShotCountDowns(ref weaponRuntimeHandlers, nextShotCountDowns);
        }
    }
}