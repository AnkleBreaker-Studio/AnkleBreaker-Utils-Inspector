using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FreeRangeAttribute))]
    public class FreeRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FreeRangeAttribute range = (FreeRangeAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.Float &&
                property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use FreeRange with float or int.");
                return;
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float fieldWidth = 50f;
            float spacing = 5f;
            Rect sliderRect = new Rect(position.x, position.y, position.width - fieldWidth - spacing, position.height);
            Rect fieldRect = new Rect(position.x + position.width - fieldWidth, position.y, fieldWidth, position.height);

            float currentValue = property.propertyType == SerializedPropertyType.Integer
                ? property.intValue
                : property.floatValue;

            float visualSliderValue = Mathf.Clamp(currentValue, range.Min, range.Max);

            EditorGUI.BeginChangeCheck();
            float newSliderValue = GUI.HorizontalSlider(sliderRect, visualSliderValue, range.Min, range.Max);
            if (EditorGUI.EndChangeCheck())
                currentValue = newSliderValue;

            EditorGUI.BeginChangeCheck();
            float newFieldValue = EditorGUI.FloatField(fieldRect, currentValue);
            if (EditorGUI.EndChangeCheck())
                currentValue = newFieldValue;

            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = Mathf.RoundToInt(currentValue);
            else
                property.floatValue = currentValue;
        }
    }
}
