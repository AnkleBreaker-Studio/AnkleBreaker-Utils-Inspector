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

        /// <summary>
        /// When true, ABGroupedEditor has already drawn this header manually.
        /// The decorator skips its draw and resets the flag.
        /// </summary>
        internal static bool SuppressNextDraw;

        public override float GetHeight()
        {
            if (SuppressNextDraw) return 0f;
            return TopPadding + LabelHeight + LineHeight + BottomPadding;
        }

        public override void OnGUI(Rect position)
        {
            if (SuppressNextDraw)
            {
                SuppressNextDraw = false;
                return;
            }

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

        /// <summary>
        /// Draw a section header using EditorGUILayout (for ABGroupedEditor manual draw).
        /// </summary>
        internal static void DrawManualSectionHeader(string title)
        {
            EditorGUILayout.Space(TopPadding);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                normal = { textColor = EditorStyles.boldLabel.normal.textColor }
            };
            EditorGUILayout.LabelField(title, headerStyle);

            Rect lineRect = EditorGUILayout.GetControlRect(false, LineHeight);
            Color lineColor = EditorGUIUtility.isProSkin
                ? new Color(0.4f, 0.4f, 0.4f)
                : new Color(0.6f, 0.6f, 0.6f);
            EditorGUI.DrawRect(lineRect, lineColor);

            EditorGUILayout.Space(BottomPadding);
        }
    }
}
