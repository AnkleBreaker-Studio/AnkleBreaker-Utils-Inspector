using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Draws a serialized list/array as a compact table with columns for each field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TableListAttribute : PropertyAttribute
    {
        public bool ShowPaging { get; private set; }
        public int MaxItemsPerPage { get; private set; }

        /// <param name="showPaging">Show page navigation for large lists. Default: true.</param>
        /// <param name="maxItemsPerPage">Items per page. Default: 20.</param>
        public TableListAttribute(bool showPaging = true, int maxItemsPerPage = 20)
        {
            ShowPaging = showPaging;
            MaxItemsPerPage = maxItemsPerPage;
        }
    }
}
