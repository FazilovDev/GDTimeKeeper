using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.General
{
    public static class ArrayChecker
    {
        public static bool AreAllTrue(bool[] isTrueArray)
        {
            for (int i = 0; i < isTrueArray.Length; i++)
            {
                if (!isTrueArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAllFalse(bool[] isTrueArray)
        {
            for (int i = 0; i < isTrueArray.Length; i++)
            {
                if (isTrueArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAllZeroOrLess(float[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > 0f)
                {
                    return false;
                }
            }

            return true;
        }

        public static int NumberOfTrueStates(bool[] isTrueArray)
        {
            int trueCount = 0;

            for (int i = 0; i < isTrueArray.Length; i++)
            {
                if (isTrueArray[i])
                {
                    trueCount++;
                }
            }

            return trueCount;
        }

        public static int NumberOfFalseStates(bool[] isTrueArray)
        {
            int falseCount = 0;

            for (int i = 0; i < isTrueArray.Length; i++)
            {
                if (!isTrueArray[i])
                {
                    falseCount++;
                }
            }

            return falseCount;
        }


        // ? Class would need spliting in future
    }
}