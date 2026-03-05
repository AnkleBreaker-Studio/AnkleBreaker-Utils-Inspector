using System;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Marks a method to be called when the inspector is closed/disabled (OnDisable).
    /// The method must be parameterless.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class OnInspectorDisposeAttribute : Attribute { }
}
