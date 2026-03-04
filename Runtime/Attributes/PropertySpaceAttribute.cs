using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Adds vertical spacing before and/or after a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class PropertySpaceAttribute : PropertyAttribute
    {
        public float SpaceBefore { get; private set; }
        public float SpaceAfter { get; private set; }

        public PropertySpaceAttribute(float spaceBefore = 8f, float spaceAfter = 0f)
        {
            SpaceBefore = spaceBefore;
            SpaceAfter = spaceAfter;
        }
    }
}
