using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Makes a field delayed: the value is only applied when the user presses Enter
    /// or the field loses focus. Works with string, int, and float fields.
    /// Similar to Unity's built-in [Delayed] but works as a PropertyAttribute
    /// that can be combined with other custom attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DelayedPropertyAttribute : PropertyAttribute
    {
    }
}
