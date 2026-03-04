using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Invokes a callback method whenever the field value changes in the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public string MethodName { get; private set; }

        /// <param name="methodName">Name of a parameterless method to call on value change.</param>
        public OnValueChangedAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
