using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    public enum FoldoutGroupStyle
    {
        /// <summary>Standard Unity foldout header (default).</summary>
        Default,
        /// <summary>Bold label + thin separator line below the title.</summary>
        Line,
        /// <summary>Title centered between two horizontal lines — Title —</summary>
        CenterLine,
        /// <summary>Full-width dark/light box background behind the title.</summary>
        Box,
        /// <summary>Clean: bold label only, no extra decoration.</summary>
        Clean
    }

    /// <summary>
    /// Groups fields inside a collapsible foldout.
    /// All fields with the same group name are drawn together.
    /// Supports multiple visual styles and optional custom color.
    /// </summary>
    /// <example><code>
    /// [FoldoutGroup("Movement")]
    /// public float speed;
    ///
    /// [FoldoutGroup("Movement")]
    /// public float jumpForce;
    ///
    /// [FoldoutGroup("Events", FoldoutGroupStyle.Box, 0.2f, 0.6f, 1f)]
    /// public UnityEvent onJump;
    /// </code></example>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FoldoutGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }
        public FoldoutGroupStyle Style { get; private set; }
        public bool HasCustomColor { get; private set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        public FoldoutGroupAttribute(string groupName, FoldoutGroupStyle style = FoldoutGroupStyle.Default)
        {
            GroupName = groupName;
            Style = style;
        }

        public FoldoutGroupAttribute(string groupName, FoldoutGroupStyle style, float r, float g, float b)
        {
            GroupName = groupName;
            Style = style;
            HasCustomColor = true;
            R = r; G = g; B = b;
        }
    }
}
