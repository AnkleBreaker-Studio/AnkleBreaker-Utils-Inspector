using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SectionHeaderAttribute))]
    public class SectionHeaderDrawer : DecoratorDrawer
    {
        private const float TopPadding = 8f;
        private const float BottomPadding = 4f;
        private const float LineHeight = 1f;
        private const float LabelHeight = 16f;

        public override float GetHeight()
        {
            return TopPadding + LabelHeight + LineHeight + BottomPadding;
        }

        public override void OnGUI(Rect position)
        {
            SectionHeaderAttribute attr = (SectionHeaderAttribute)attribute;

            position.y += TopPadding;

            // Bold label
            Rect labelRect = new Rect(position.x, position.y, position.width, LabelHeight);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                normal = { textColor = EditorStyles.boldLabel.normal.textColor }
            };
            EditorGUI.LabelField(labelRect, attr.Title, headerStyle);

            // Separator line
            Rect lineRect = new Rect(position.x, position.y + LabelHeight, position.width, LineHeight);
            Color lineColor = EditorGUIUtility.isProSkin
                ? new Color(0.4f, 0.4f, 0.4f)
                : new Color(0.6f, 0.6f, 0.6f);
            EditorGUI.DrawRect(lineRect, lineColor);
        }
    }
}
