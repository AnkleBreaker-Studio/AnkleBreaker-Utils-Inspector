using System;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Shows a non-serialized field or property in the Inspector (read-only).
    /// Requires the class to use ABEditor or the built-in ButtonMonoBehaviourEditor.
    /// </summary>
    /// <param name="runtimeOnly">When true, the field/property is only visible in Play mode.</param>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ShowInInspectorAttribute : Attribute
    {
        /// <summary>When true, the entry is only visible during Play mode.</summary>
        public bool RuntimeOnly { get; }

        public ShowInInspectorAttribute() { }

        public ShowInInspectorAttribute(bool runtimeOnly)
        {
            RuntimeOnly = runtimeOnly;
        }
    }
}
