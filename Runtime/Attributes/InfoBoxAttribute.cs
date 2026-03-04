using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a HelpBox above the field, optionally conditional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public string Message { get; private set; }
        public InfoMessageType Type { get; private set; }
        public string VisibleIf { get; private set; }

        /// <param name="message">Message to display.</param>
        /// <param name="type">Message type (Info, Warning, Error).</param>
        /// <param name="visibleIf">Optional bool condition name. If null, always visible.</param>
        public InfoBoxAttribute(string message, InfoMessageType type = InfoMessageType.Info, string visibleIf = null)
        {
            Message = message;
            Type = type;
            VisibleIf = visibleIf;
        }
    }

    public enum InfoMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}
