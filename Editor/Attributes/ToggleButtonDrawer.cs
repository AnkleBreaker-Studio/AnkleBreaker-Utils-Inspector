using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
    public class ToggleButtonDrawer : PropertyDrawer
    {
        private static readonly Color DefaultActiveColor = new Color(0.6f, 0.9f, 0.65f, 1f);
        private static readonly Color InactiveColor = new Color(0.9f, 0.6f, 0.6f, 1f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Use ToggleButton with bool.");
                return;
            }

            ToggleButtonAttribute toggle = (ToggleButtonAttribute)attribute;

            string trueLabel = string.IsNullOrEmpty(toggle.TrueLabel) ? property.displayName : toggle.TrueLabel;
            string falseLabel = string.IsNullOrEmpty(toggle.FalseLabel) ? trueLabel : toggle.FalseLabel;
            string baseText = property.boolValue ? trueLabel : falseLabel;
            string stateTag = property.boolValue ? " (ON)" : " (OFF)";
            string buttonText = baseText + stateTag;

            Rect buttonRect = position;
            if (!toggle.FullWidth)
                buttonRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = property.boolValue ? DefaultActiveColor : InactiveColor;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = property.boolValue ? FontStyle.Bold : FontStyle.Normal
            };

            if (GUI.Button(buttonRect, buttonText, buttonStyle))
            {
                property.boolValue = !property.boolValue;
            }

            GUI.backgroundColor = previousColor;
        }
    }
}
