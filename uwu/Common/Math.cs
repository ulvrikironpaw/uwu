using UnityEngine;

namespace UWU.Common
{
    internal static class Math
    {
        internal static float LerpStep(float a, float b, float t)
        {
            if (t <= a) return 0f;
            if (t >= b) return 1f;
            return Mathf.InverseLerp(a, b, t);
        }
    }
}