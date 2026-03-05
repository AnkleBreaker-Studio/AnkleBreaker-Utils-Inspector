using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    public enum SectionHeaderStyle
    {
        /// <summary>Bold label + thin line below (default, current behavior).</summary>
        Line,
        /// <summary>Title centered between two horizontal lines — Title —</summary>
        CenterLine,
        /// <summary>Full-width dark/light box background behind the title.</summary>
        Box,
        /// <summary>Clean: bold label only, no line, just spacing.</summary>
        Clean
    }

    /// <summary>
    /// Draws a styled separator with a title above the field.
    /// Usage: [SectionHeader("My Section")] or [SectionHeader("My Section", SectionHeaderStyle.Box)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class SectionHeaderAttribute : PropertyAttribute
    {
        public string Title { get; private set; }
        public SectionHeaderStyle Style { get; private set; }
        public bool HasCustomColor { get; private set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        public SectionHeaderAttribute(string title, SectionHeaderStyle style = SectionHeaderStyle.CenterLine)
        {
            Title = title;
            Style = style;
        }

        public SectionHeaderAttribute(string title, float r, float g, float b)
        {
            Title = title;
            Style = SectionHeaderStyle.CenterLine;
            HasCustomColor = true;
            R = r; G = g; B = b;
        }

        public SectionHeaderAttribute(string title, SectionHeaderStyle style, float r, float g, float b)
        {
            Title = title;
            Style = style;
            HasCustomColor = true;
            R = r; G = g; B = b;
        }
    }
}
