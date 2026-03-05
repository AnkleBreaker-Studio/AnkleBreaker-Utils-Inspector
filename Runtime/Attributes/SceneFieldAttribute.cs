using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Marks a string field as a scene reference.
    /// Displays a scene asset picker and validates against Build Settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SceneFieldAttribute : PropertyAttribute { }
}
