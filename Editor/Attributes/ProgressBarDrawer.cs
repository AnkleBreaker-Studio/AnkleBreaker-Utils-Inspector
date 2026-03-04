using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ProgressBarAttribute attr = (ProgressBarAttribute)attribute;

            float value = property.propertyType == SerializedPropertyType.Integer
                ? property.intValue
                : property.floatValue;

            float fraction = Mathf.Clamp01((value - attr.Min) / (attr.Max - attr.Min));

            // Label prefix
            position = EditorGUI.PrefixLabel(position, label);

            // Background
            EditorGUI.DrawRect(position, new Color(0.15f, 0.15f, 0.15f));

            // Filled bar
            Rect fillRect = new Rect(position.x, position.y, position.width * fraction, position.height);
            EditorGUI.DrawRect(fillRect, new Color(attr.R, attr.G, attr.B));

            // Text overlay
            string displayText = string.IsNullOrEmpty(attr.Label)
                ? $"{value:F1} / {attr.Max:F1}"
                : $"{attr.Label}: {value:F1} / {attr.Max:F1}";

            GUIStyle centeredStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                fontStyle = FontStyle.Bold
            };

            EditorGUI.LabelField(position, displayText, centeredStyle);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
