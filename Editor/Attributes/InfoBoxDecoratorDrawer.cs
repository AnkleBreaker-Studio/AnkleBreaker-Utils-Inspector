using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDecoratorDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            InfoBoxAttribute attr = (InfoBoxAttribute)attribute;

            if (!string.IsNullOrEmpty(attr.VisibleIf))
                return GetHeightInternal(attr);

            return GetMessageHeight(attr.Message) + 4f;
        }

        public override void OnGUI(Rect position)
        {
            InfoBoxAttribute attr = (InfoBoxAttribute)attribute;

            // Note: DecoratorDrawer doesn't have access to the property.
            // VisibleIf condition cannot be evaluated here without the property.
            // For conditional InfoBox, use as PropertyDrawer pattern instead.
            // When VisibleIf is null, always show.

            float msgHeight = GetMessageHeight(attr.Message);
            Rect boxRect = new Rect(position.x, position.y, position.width, msgHeight);
            EditorGUI.HelpBox(boxRect, attr.Message, (MessageType)attr.Type);
        }

        private float GetHeightInternal(InfoBoxAttribute attr)
        {
            return GetMessageHeight(attr.Message) + 4f;
        }

        private static float GetMessageHeight(string message)
        {
            return Mathf.Max(
                EditorStyles.helpBox.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth),
                EditorGUIUtility.singleLineHeight * 1.5f);
        }
    }
}
