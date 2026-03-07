using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays the field value as a read-only text label instead of an editable input field.
    /// Useful for showing computed values, IDs, or debug info directly in the inspector.
    /// </summary>
    /// <example><code>
    /// [DisplayAsString]
    /// public string uniqueId = "abc-123";
    ///
    /// [DisplayAsString]
    /// public int frameCount;
    /// </code></example>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DisplayAsStringAttribute : PropertyAttribute
    {
        /// <summary>If true, the text is displayed without a label (full width).</summary>
        public bool HideLabel { get; set; }

        /// <summary>Font size override. 0 = default.</summary>
        public int FontSize { get; set; }
    }
}
