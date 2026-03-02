using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    public class HelpBoxAttribute : PropertyAttribute
    {
        public string Message;
        public MessageType Type;

        public HelpBoxAttribute(string message, MessageType type = MessageType.Info)
        {
            this.Message = message;
            this.Type = type;
        }

        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error
        }
    }
}