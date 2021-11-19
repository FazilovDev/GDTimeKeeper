using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.WeaponSpace.Input
{
    public class Manager
    {
        public KeyCode FireInput { get; set; }
        public KeyCode AutoFireInput { get; set; }
        public KeyCode ReloadInput { get; set; }
        public KeyCode SwitchInput { get; set; }
        public KeyCode AimInput { get; set; }
        public KeyCode RunInput { get; set; }

        public bool IsFireDetected { get; private set; }
        public bool IsAutoFireDetected { get; private set; }
        public bool IsReloadDetected { get; private set; }
        public bool IsSwitchDetected { get; private set; }
        public bool IsAimDetected { get; private set; }
        public bool IsRunDetected { get; private set; }

        public Manager(
            KeyCode fireInput,
            KeyCode autoFireInput,
            KeyCode reloadInput,
            KeyCode switchInput,
            KeyCode aimInput,
            KeyCode runInput
            )
        {
            FireInput = fireInput;
            AutoFireInput = autoFireInput;
            ReloadInput = reloadInput;
            SwitchInput = switchInput;
            AimInput = aimInput;
            RunInput = runInput;
        }

        public void Update()
        {
            IsFireDetected = UnityEngine.Input.GetKeyDown(FireInput);
            IsAutoFireDetected = UnityEngine.Input.GetKey(AutoFireInput);
            IsReloadDetected = UnityEngine.Input.GetKeyDown(ReloadInput);
            IsSwitchDetected = UnityEngine.Input.GetKeyDown(SwitchInput);
            IsAimDetected = UnityEngine.Input.GetKey(AimInput);
            IsRunDetected = UnityEngine.Input.GetKey(RunInput);
        }
    }
}