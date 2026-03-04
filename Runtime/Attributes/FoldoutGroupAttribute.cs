using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Groups fields inside a collapsible foldout.
    /// All fields with the same group name are drawn together.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FoldoutGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }

        public FoldoutGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
