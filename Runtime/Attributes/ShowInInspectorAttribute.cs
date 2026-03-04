using System;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Shows a non-serialized field or property in the Inspector (read-only).
    /// Requires the class to use ABEditor or the built-in ButtonMonoBehaviourEditor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ShowInInspectorAttribute : Attribute
    {
    }
}
