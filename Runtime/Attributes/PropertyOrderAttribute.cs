using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Controls the display order of a field in the Inspector.
    /// Lower values appear first. Default order is 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class PropertyOrderAttribute : PropertyAttribute
    {
        public int Order { get; private set; }

        public PropertyOrderAttribute(int order)
        {
            Order = order;
            // Also set Unity's built-in order
            this.order = order;
        }
    }
}
