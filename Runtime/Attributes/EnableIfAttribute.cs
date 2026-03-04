using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnableIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }
        public object CompareValue { get; private set; }
        public bool HasCompareValue { get; private set; }

        /// <summary>
        /// Enable the field when the boolean condition is true. Grayed out otherwise.
        /// </summary>
        public EnableIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
            HasCompareValue = false;
        }

        /// <summary>
        /// Enable the field when the target field equals the given int value.
        /// Works with enums cast to int (e.g. (int)MyEnum.Value).
        /// </summary>
        public EnableIfAttribute(string fieldName, int compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }

        /// <summary>
        /// Enable the field when the target field equals the given string value.
        /// </summary>
        public EnableIfAttribute(string fieldName, string compareValue)
        {
            ConditionName = fieldName;
            CompareValue = compareValue;
            HasCompareValue = true;
        }
    }
}
