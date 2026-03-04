using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Disables (grays out) the field during Play Mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DisableInPlayModeAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Disables (grays out) the field in Editor Mode (only editable during Play Mode).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DisableInEditorModeAttribute : PropertyAttribute
    {
    }
}
