using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Wraps a numeric value within a cyclic range [Min, Max).
    /// For example, [Wrap(0, 360)] will wrap 361 to 1, and -1 to 359.
    /// Works with int and float fields.
    /// </summary>
    /// <example><code>
    /// [Wrap(0f, 360f)]
    /// public float angle;
    ///
    /// [Wrap(0, 24)]
    /// public int hour;
    /// </code></example>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class WrapAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public WrapAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
