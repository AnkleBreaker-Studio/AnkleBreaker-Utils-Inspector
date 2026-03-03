using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a slider with min/max range, but allows typing values beyond the range in the text field.
    /// Works with float and int fields.
    /// </summary>
    /// <example><code>
    /// [FreeRange(0f, 100f)]
    /// public float speed = 50f;
    /// </code></example>
    public class FreeRangeAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public FreeRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
