using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays an enum field as a row of toggle buttons instead of a dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumToggleButtonsAttribute : PropertyAttribute
    {
    }
}
