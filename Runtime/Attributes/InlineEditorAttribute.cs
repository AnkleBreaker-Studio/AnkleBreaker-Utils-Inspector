using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws the referenced ScriptableObject or Component's inspector inline,
    /// allowing editing without switching to the asset.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InlineEditorAttribute : PropertyAttribute
    {
        public bool DrawHeader { get; private set; }

        /// <param name="drawHeader">Whether to draw the object header. Default: true.</param>
        public InlineEditorAttribute(bool drawHeader = true)
        {
            DrawHeader = drawHeader;
        }
    }
}
