using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws fields with the same group name horizontally side by side.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HorizontalGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }
        public float Width { get; private set; }

        /// <param name="groupName">Identifier for the horizontal group.</param>
        /// <param name="width">Relative width (0 = auto). Default: 0.</param>
        public HorizontalGroupAttribute(string groupName, float width = 0f)
        {
            GroupName = groupName;
            Width = width;
        }
    }
}
