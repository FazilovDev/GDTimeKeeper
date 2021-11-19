using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.General.ErrorHandling
{
    public static class NullChecker
    {
        // Use with conditions and if initialised instances are optional
        public static bool IsNull(Object objectToCheck)
        {
            if (objectToCheck == null)
            {
                return true;
            }

            return false;
        }

        // Use as standalone call and if initialised instances are essential
        public static void ThrowIfNull(Object objectToCheck, string objectName, string instructiveText = "")
        {
            if (objectToCheck != null)
            {
                return;
            }

            ErrorMessenger.ThrowException("The instance named '" + objectName + "' must not be null.", instructiveText);
        }
    }
}