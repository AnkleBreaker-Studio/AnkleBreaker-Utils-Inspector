using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    public class ABToolTipAttribute : PropertyAttribute
    {
        public string TooltipMemberName { get; }

        public ABToolTipAttribute(string tooltipMemberName)
        {
            TooltipMemberName = tooltipMemberName;
        }
    }
}