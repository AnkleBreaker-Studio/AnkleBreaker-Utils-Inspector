using System;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws a small button on the same line as the property field.
    /// The button invokes the specified method on the target object.
    /// </summary>
    /// <example><code>
    /// [InlineButton("GenerateGUID", "Gen")]
    /// public string guid;
    ///
    /// [InlineButton("Randomize", "\u21bb")]
    /// public float speed = 5f;
    ///
    /// private void GenerateGUID() => guid = System.Guid.NewGuid().ToString();
    /// private void Randomize() => speed = Random.Range(0f, 100f);
    /// </code></example>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class InlineButtonAttribute : PropertyAttribute
    {
        /// <summary>Name of the method to invoke when the button is clicked.</summary>
        public readonly string MethodName;

        /// <summary>Custom label displayed on the button. Defaults to the method name if null.</summary>
        public readonly string Label;

        /// <summary>
        /// Controls when the button is interactable.
        /// Defaults to <see cref="ButtonMode.AlwaysEnabled"/>.
        /// </summary>
        public ButtonMode Mode { get; set; } = ButtonMode.AlwaysEnabled;

        /// <summary>Fixed width of the button in pixels. 0 = auto-size to label.</summary>
        public float Width { get; set; }

        public InlineButtonAttribute(string methodName, string label = null)
        {
            MethodName = methodName;
            Label = label;
        }
    }
}
