using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Groups fields inside a styled box with a title.
    /// All fields with the same group name are drawn together.
    /// Optionally hide/show the entire box based on a condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class BoxGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }
        /// <summary>When true, the box can be collapsed with a foldout arrow.</summary>
        public bool Foldable { get; private set; }
        /// <summary>Name of a bool field, property, or method controlling visibility of the entire box.</summary>
        public string ShowIf { get; set; }

        public BoxGroupAttribute(string groupName, bool foldable = false)
        {
            GroupName = groupName;
            Foldable = foldable;
        }
    }
}
