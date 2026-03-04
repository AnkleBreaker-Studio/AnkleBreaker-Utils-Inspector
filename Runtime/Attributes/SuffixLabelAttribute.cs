using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a suffix label after the field (e.g. "ms", "px", "%").
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SuffixLabelAttribute : PropertyAttribute
    {
        public string Suffix { get; private set; }

        public SuffixLabelAttribute(string suffix)
        {
            Suffix = suffix;
        }
    }
}
