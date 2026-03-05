using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SectionHeaderAttribute))]
    public class SectionHeaderDrawer : DecoratorDrawer
    {
        private const float TopPadding = 8f;
        private const float BottomPadding = 4f;
        private const float LineThickness = 1f;
        private const float LabelHeight = 18f;
        private const float BoxPadding = 4f;

        internal static bool SuppressNextDraw;

        private SectionHeaderAttribute Attr => (SectionHeaderAttribute)attribute;

        public override float GetHeight()
        {
            if (SuppressNextDraw) return 0f;
            switch (Attr.Style)
            {
                case SectionHeaderStyle.Box:
                    return TopPadding + BoxPadding * 2 + LabelHeight + BottomPadding;
                case SectionHeaderStyle.Clean:
                    return TopPadding + LabelHeight + BottomPadding;
                default: // Line, CenterLine
                    return TopPadding + LabelHeight + LineThickness + BottomPadding;
            }
        }

        public override void OnGUI(Rect position)
        {
            if (SuppressNextDraw) { SuppressNextDraw = false; return; }
            DrawRect(position, Attr.Title, Attr.Style);
        }

        /// <summary>Rect-based draw used by both the DecoratorDrawer and manual layout path.</summary>
        internal static void DrawRect(Rect position, string title, SectionHeaderStyle style)
        {
            position.y += TopPadding;
            Color line = EditorGUIUtility.isProSkin ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f);
            GUIStyle bold = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };

            switch (style)
            {
                case SectionHeaderStyle.Line:
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LabelHeight), title, bold);
                    EditorGUI.DrawRect(new Rect(position.x, position.y + LabelHeight, position.width, LineThickness), line);
                    break;

                case SectionHeaderStyle.CenterLine:
                    float textW = bold.CalcSize(new GUIContent(title)).x + 12f;
                    float sideW = (position.width - textW) * 0.5f;
                    float lineY = position.y + LabelHeight * 0.5f;
                    if (sideW > 4f)
                    {
                        EditorGUI.DrawRect(new Rect(position.x, lineY, sideW - 4f, LineThickness), line);
                        EditorGUI.DrawRect(new Rect(position.x + sideW + textW + 4f, lineY, sideW - 4f, LineThickness), line);
                    }
                    GUIStyle center = new GUIStyle(bold) { alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LabelHeight), title, center);
                    break;

                case SectionHeaderStyle.Box:
                    Color bg = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.82f, 0.82f, 0.82f);
                    float boxH = LabelHeight + BoxPadding * 2;
                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, boxH), bg);
                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, LineThickness), line);
                    EditorGUI.DrawRect(new Rect(position.x, position.y + boxH - LineThickness, position.width, LineThickness), line);
                    GUIStyle boxLabel = new GUIStyle(bold) { alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(new Rect(position.x, position.y + BoxPadding, position.width, LabelHeight), title, boxLabel);
                    break;

                case SectionHeaderStyle.Clean:
                    GUIStyle cleanStyle = new GUIStyle(bold) { fontSize = 13 };
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LabelHeight), title, cleanStyle);
                    break;
            }
        }

        /// <summary>Layout-based draw for ABGroupedEditor manual path.</summary>
        internal static void DrawManualSectionHeader(string title, SectionHeaderStyle style = SectionHeaderStyle.Line)
        {
            float h = TopPadding + LabelHeight + BottomPadding;
            if (style == SectionHeaderStyle.Box) h = TopPadding + BoxPadding * 2 + LabelHeight + BottomPadding;
            else if (style != SectionHeaderStyle.Clean) h = TopPadding + LabelHeight + LineThickness + BottomPadding;

            Rect rect = EditorGUILayout.GetControlRect(false, h);
            DrawRect(rect, title, style);
        }
    }
}
