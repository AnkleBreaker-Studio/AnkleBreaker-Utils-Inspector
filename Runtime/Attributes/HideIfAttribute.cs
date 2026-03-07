using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }
        public object CompareValue { get; private set; }
        public bool HasCompareValue { get; private set; }

        /// <summary>Hide the field when the boolean condition is true.</summary>
        public HideIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
            HasCompareValue = false;
        }

        /// <summary>Hide the field when the target field equals the given bool value.</summary>
        public HideIfAttribute(string fieldName, bool compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }

        /// <summary>Hide the field when the target field equals the given int value (works with enums cast to int).</summary>
        public HideIfAttribute(string fieldName, int compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }

        /// <summary>Hide the field when the target field equals the given float value.</summary>
        public HideIfAttribute(string fieldName, float compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }

        /// <summary>Hide the field when the target field equals the given string value.</summary>
        public HideIfAttribute(string fieldName, string compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }
    }
}
