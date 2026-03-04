using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxSliderAttribute attr = (MinMaxSliderAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.LabelField(position, label.text, "[MinMaxSlider] requires Vector2");
                return;
            }

            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;

            position = EditorGUI.PrefixLabel(position, label);

            float fieldWidth = 50f;
            float sliderPadding = 5f;

            Rect minRect = new Rect(position.x, position.y, fieldWidth, position.height);
            Rect sliderRect = new Rect(position.x + fieldWidth + sliderPadding, position.y,
                position.width - fieldWidth * 2 - sliderPadding * 2, position.height);
            Rect maxRect = new Rect(position.xMax - fieldWidth, position.y, fieldWidth, position.height);

            min = EditorGUI.FloatField(minRect, min);
            EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, attr.Min, attr.Max);
            max = EditorGUI.FloatField(maxRect, max);

            min = Mathf.Clamp(min, attr.Min, max);
            max = Mathf.Clamp(max, min, attr.Max);

            property.vector2Value = new Vector2(min, max);
        }
    }
}
