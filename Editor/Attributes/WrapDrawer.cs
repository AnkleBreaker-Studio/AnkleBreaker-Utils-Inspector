using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(WrapAttribute))]
    public class WrapDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (WrapAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            if (property.propertyType == SerializedPropertyType.Float)
            {
                float value = EditorGUI.FloatField(position, label, property.floatValue);
                if (EditorGUI.EndChangeCheck())
                    property.floatValue = Wrap(value, attr.Min, attr.Max);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                int value = EditorGUI.IntField(position, label, property.intValue);
                if (EditorGUI.EndChangeCheck())
                    property.intValue = WrapInt(value, (int)attr.Min, (int)attr.Max);
            }
            else
            {
                EditorGUI.EndChangeCheck();
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }

        private static float Wrap(float value, float min, float max)
        {
            float range = max - min;
            if (range <= 0f) return min;
            float result = ((value - min) % range);
            if (result < 0f) result += range;
            return result + min;
        }

        private static int WrapInt(int value, int min, int max)
        {
            int range = max - min;
            if (range <= 0) return min;
            int result = ((value - min) % range);
            if (result < 0) result += range;
            return result + min;
        }
    }
}
