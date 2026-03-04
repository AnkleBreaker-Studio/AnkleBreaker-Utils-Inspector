using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws a small button next to the field that invokes the specified method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InlineButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; private set; }
        public string Label { get; private set; }

        /// <param name="methodName">Name of a parameterless method on the same class.</param>
        /// <param name="label">Button label. If null, uses the method name.</param>
        public InlineButtonAttribute(string methodName, string label = null)
        {
            MethodName = methodName;
            Label = label;
        }
    }
}
