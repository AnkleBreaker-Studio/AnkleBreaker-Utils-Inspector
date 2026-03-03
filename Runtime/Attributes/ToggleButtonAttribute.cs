using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a boolean field as a toggle button instead of a checkbox.
    /// The button shows the label text and changes color based on the value.
    /// </summary>
    /// <example><code>
    /// [ToggleButton]
    /// public bool isActive;
    /// 
    /// [ToggleButton("Enable Debug Mode")]
    /// public bool debugMode;
    /// 
    /// [ToggleButton("Enabled", "Disabled")]
    /// public bool featureToggle;
    /// </code></example>
    public class ToggleButtonAttribute : PropertyAttribute
    {
        /// <summary>Text shown when the value is true. Defaults to the field name.</summary>
        public string TrueLabel { get; private set; }

        /// <summary>Text shown when the value is false. If null, uses TrueLabel for both states.</summary>
        public string FalseLabel { get; private set; }

        public ToggleButtonAttribute(string trueLabel = null, string falseLabel = null)
        {
            TrueLabel = trueLabel;
            FalseLabel = falseLabel;
        }
    }
}
