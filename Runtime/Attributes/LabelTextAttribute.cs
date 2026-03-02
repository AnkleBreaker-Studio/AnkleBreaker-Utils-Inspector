using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class LabelTextAttribute : PropertyAttribute
    {
        public string DisplayName { get; }

        public LabelTextAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}