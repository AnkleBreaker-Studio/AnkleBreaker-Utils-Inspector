using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a string field with a file browser button.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FilePathAttribute : PropertyAttribute
    {
        /// <summary>File extension filters (e.g. "png,jpg,gif" or "cs"). Empty = all files.</summary>
        public string Extensions { get; }

        /// <summary>When true, stores the absolute path. Otherwise stores relative to Assets/.</summary>
        public bool AbsolutePath { get; set; }

        public FilePathAttribute(string extensions = "")
        {
            Extensions = extensions;
        }
    }
}
