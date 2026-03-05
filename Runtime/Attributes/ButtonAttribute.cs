using System;

namespace AnkleBreaker.Utils.Inspector
{
    public enum ButtonMode
    {
        AlwaysEnabled,
        EnabledInPlayMode,
        DisabledInPlayMode
    }

    /// <summary>
    /// Displays a method as a clickable button in the Unity Inspector.
    /// The method can be parameterless or have default-value parameters.
    /// </summary>
    /// <example><code>
    /// [Button]
    /// public void MyMethod()
    /// {
    ///     Debug.Log("Clicked!");
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        /// <summary>Custom label for the button, or null to use the method name.</summary>
        public readonly string Name;

        public ButtonAttribute() { }

        public ButtonAttribute(string name) => Name = name;

        /// <summary>
        /// Controls when the button is interactable.
        /// Defaults to <see cref="ButtonMode.AlwaysEnabled"/>.
        /// </summary>
        public ButtonMode Mode { get; set; } = ButtonMode.AlwaysEnabled;

        /// <summary>
        /// Optional group name. Buttons sharing the same HorizontalGroup are drawn side by side.
        /// </summary>
        public string HorizontalGroup { get; set; }
    }
}