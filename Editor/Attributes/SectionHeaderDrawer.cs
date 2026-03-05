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

        private static readonly Color DefaultLine = EditorGUIUtility.isProSkin
            ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f);

        public override void OnGUI(Rect position)
        {
            if (SuppressNextDraw) { SuppressNextDraw = false; return; }
            Color? customColor = Attr.HasCustomColor ? new Color(Attr.R, Attr.G, Attr.B) : (Color?)null;
            DrawRect(position, Attr.Title, Attr.Style, customColor);
        }

        /// <summary>Rect-based draw used by both the DecoratorDrawer and manual layout path.</summary>
        internal static void DrawRect(Rect position, string title, SectionHeaderStyle style, Color? customColor = null)
        {
            position.y += TopPadding;
            Color line = customColor ?? DefaultLine;
            GUIStyle bold = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };
            if (customColor.HasValue)
                bold.normal.textColor = customColor.Value;

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
                    Color bg = customColor.HasValue
                        ? new Color(customColor.Value.r * 0.3f, customColor.Value.g * 0.3f, customColor.Value.b * 0.3f, 0.5f)
                        : (EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.82f, 0.82f, 0.82f));
                    float boxH = LabelHeight + BoxPadding * 2;
                    float bw = 2f; // border width

                    // Full background fill
                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, boxH), bg);
                    // Top border
                    EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, bw), line);
                    // Bottom border
                    EditorGUI.DrawRect(new Rect(position.x, position.y + boxH - bw, position.width, bw), line);
                    // Left border
                    EditorGUI.DrawRect(new Rect(position.x, position.y, bw, boxH), line);
                    // Right border
                    EditorGUI.DrawRect(new Rect(position.x + position.width - bw, position.y, bw, boxH), line);

                    GUIStyle boxLabel = new GUIStyle(bold) { alignment = TextAnchor.MiddleCenter };
                    if (customColor.HasValue)
                        boxLabel.normal.textColor = customColor.Value;
                    EditorGUI.LabelField(new Rect(position.x, position.y + BoxPadding, position.width, LabelHeight), title, boxLabel);
                    break;

                case SectionHeaderStyle.Clean:
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LabelHeight), title, bold);
                    break;
            }
        }

        /// <summary>Layout-based draw for ABGroupedEditor manual path.</summary>
        internal static void DrawManualSectionHeader(string title, SectionHeaderStyle style = SectionHeaderStyle.CenterLine, Color? customColor = null)
        {
            float h = TopPadding + LabelHeight + BottomPadding;
            if (style == SectionHeaderStyle.Box) h = TopPadding + BoxPadding * 2 + LabelHeight + BottomPadding;
            else if (style != SectionHeaderStyle.Clean) h = TopPadding + LabelHeight + LineThickness + BottomPadding;

            Rect rect = EditorGUILayout.GetControlRect(false, h);
            DrawRect(rect, title, style, customColor);
        }
    }
}
