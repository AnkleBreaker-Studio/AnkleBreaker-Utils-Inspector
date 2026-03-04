using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Groups fields inside a styled box with a title.
    /// All fields with the same group name are drawn together.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class BoxGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }

        public BoxGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
