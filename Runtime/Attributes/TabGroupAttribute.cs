using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Groups fields into tabs. Fields with the same TabName appear in the same tab.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TabGroupAttribute : PropertyAttribute
    {
        public string TabName { get; private set; }

        public TabGroupAttribute(string tabName)
        {
            TabName = tabName;
        }
    }
}
