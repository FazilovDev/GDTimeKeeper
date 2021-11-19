using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Utilities.General
{
    public struct Range
    {
        float _Minimum, _Maximum;

        public float RandomValue
        {
            get
            {
                return Random.Range(_Minimum, _Maximum);
            }
        }

        public Range(float minimum, float maximum)
        {
            _Minimum = minimum;
            _Maximum = maximum;
        }
    }
}
