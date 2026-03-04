using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a dropdown filled with values from a method or field returning IList.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ValueDropdownAttribute : PropertyAttribute
    {
        public string MemberName { get; private set; }

        /// <param name="memberName">Name of a field, property, or method returning IList.</param>
        public ValueDropdownAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}
