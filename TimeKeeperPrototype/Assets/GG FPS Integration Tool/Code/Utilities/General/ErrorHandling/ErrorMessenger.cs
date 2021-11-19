using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.General.ErrorHandling
{
    public static class ErrorMessenger
    {
        static string ErrorMessageIntro { get; } = "GGFPSIntegrationTool ErrorHandling: ";

        public static void ThrowException(string informativeText, string instructiveText = "")
        {
            throw new System.Exception(ErrorMessageIntro + informativeText + '\n' + instructiveText);
        }
    }
}