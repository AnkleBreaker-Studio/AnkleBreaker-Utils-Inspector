using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Validates the field value using a method that returns bool.
    /// Shows an error/warning box when validation fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ValidateInputAttribute : PropertyAttribute
    {
        public string ValidatorMethod { get; private set; }
        public string Message { get; private set; }
        public ValidateMessageType MessageType { get; private set; }

        /// <param name="validatorMethod">Method name returning bool. Can take the field value as parameter or be parameterless.</param>
        /// <param name="message">Error message when validation fails.</param>
        /// <param name="messageType">Type of message to display.</param>
        public ValidateInputAttribute(string validatorMethod, string message = "Validation failed", ValidateMessageType messageType = ValidateMessageType.Error)
        {
            ValidatorMethod = validatorMethod;
            Message = message;
            MessageType = messageType;
        }
    }

    public enum ValidateMessageType
    {
        Info = 1,
        Warning = 2,
        Error = 3
    }
}
