using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays an asset preview (texture, sprite, prefab) next to the object field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class PreviewFieldAttribute : PropertyAttribute
    {
        public float PreviewHeight { get; private set; }

        /// <param name="previewHeight">Height of the preview in pixels. Default: 64.</param>
        public PreviewFieldAttribute(float previewHeight = 64f)
        {
            PreviewHeight = previewHeight;
        }
    }
}
