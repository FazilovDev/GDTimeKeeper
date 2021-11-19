using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.General
{
    public static class ArrayConverter
    {
        public static int ElementToIndex<T>(T targetElement, List<T> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (targetElement.Equals(elements[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}