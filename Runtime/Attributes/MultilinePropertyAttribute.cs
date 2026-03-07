using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a string field as a multiline text area with configurable line count.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class MultilinePropertyAttribute : PropertyAttribute
    {
        public int Lines { get; }

        public MultilinePropertyAttribute(int lines = 3)
        {
            Lines = Mathf.Max(1, lines);
        }
    }
}
