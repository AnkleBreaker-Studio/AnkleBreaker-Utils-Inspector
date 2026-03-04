using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws a styled separator line with a title above the field.
    /// Usage: [SectionHeader("My Section")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class SectionHeaderAttribute : PropertyAttribute
    {
        public string Title { get; private set; }

        public SectionHeaderAttribute(string title)
        {
            Title = title;
        }
    }
}
