using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }

        public ShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }

}