using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ProgressBarAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }
        public string Label { get; private set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        /// <summary>
        /// Displays a float/int field as a colored progress bar.
        /// </summary>
        /// <param name="min">Minimum value (left edge).</param>
        /// <param name="max">Maximum value (right edge).</param>
        /// <param name="label">Optional label displayed on the bar. Leave empty to show the value.</param>
        /// <param name="r">Bar color red channel (0-1). Default: 0.2</param>
        /// <param name="g">Bar color green channel (0-1). Default: 0.6</param>
        /// <param name="b">Bar color blue channel (0-1). Default: 0.9</param>
        public ProgressBarAttribute(float min, float max, string label = "", float r = 0.2f, float g = 0.6f, float b = 0.9f)
        {
            Min = min;
            Max = max;
            Label = label;
            R = r;
            G = g;
            B = b;
        }
    }
}
