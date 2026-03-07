using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Customizes how a list/array is drawn in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ListDrawerSettingsAttribute : PropertyAttribute
    {
        /// <summary>Show add/remove buttons (default: true).</summary>
        public bool ShowAddRemoveButtons { get; set; } = true;
        /// <summary>Allow drag-to-reorder (default: true).</summary>
        public bool Draggable { get; set; } = true;
        /// <summary>Show element count in header (default: true).</summary>
        public bool ShowCount { get; set; } = true;
        /// <summary>Minimum number of elements (0 = no minimum).</summary>
        public int MinCount { get; set; } = 0;
        /// <summary>Maximum number of elements (0 = no maximum).</summary>
        public int MaxCount { get; set; } = 0;
        /// <summary>Custom label format for elements. Use $index for the index number.</summary>
        public string ElementLabel { get; set; }
    }
}
