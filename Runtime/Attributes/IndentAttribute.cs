using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Controls the indentation level of a field in the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class IndentAttribute : PropertyAttribute
    {
        public int Level { get; }

        public IndentAttribute(int level = 1)
        {
            Level = level;
        }
    }
}
