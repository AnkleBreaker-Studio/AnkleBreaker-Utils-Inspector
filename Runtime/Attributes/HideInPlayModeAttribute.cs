using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Hides the field during Play Mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HideInPlayModeAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Hides the field in Editor Mode (only visible during Play Mode).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HideInEditorModeAttribute : PropertyAttribute
    {
    }
}
