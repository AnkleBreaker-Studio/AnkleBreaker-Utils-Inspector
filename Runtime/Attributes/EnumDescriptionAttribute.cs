using System;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}