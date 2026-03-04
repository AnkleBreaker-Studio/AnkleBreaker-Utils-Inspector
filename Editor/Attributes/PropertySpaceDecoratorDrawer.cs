using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(PropertySpaceAttribute))]
    public class PropertySpaceDecoratorDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            PropertySpaceAttribute attr = (PropertySpaceAttribute)attribute;
            return attr.SpaceBefore + attr.SpaceAfter;
        }

        public override void OnGUI(Rect position)
        {
            // Space is handled by GetHeight - nothing to draw
        }
    }
}
