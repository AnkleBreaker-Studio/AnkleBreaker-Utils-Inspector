using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays an enum field with a searchable popup window.
    /// Useful for enums with many values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SearchableEnumAttribute : PropertyAttribute { }
}
