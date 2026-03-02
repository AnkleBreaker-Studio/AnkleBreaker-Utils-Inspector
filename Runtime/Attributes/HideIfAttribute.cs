using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }

        public HideIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }

}