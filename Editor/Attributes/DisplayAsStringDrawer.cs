using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(DisplayAsStringAttribute))]
    public class DisplayAsStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (DisplayAsStringAttribute)attribute;
            string text = GetValueAsString(property);

            GUIStyle style = new GUIStyle(EditorStyles.label);
            if (attr.FontSize > 0)
                style.fontSize = attr.FontSize;

            if (attr.HideLabel)
                EditorGUI.LabelField(position, text, style);
            else
                EditorGUI.LabelField(position, label, new GUIContent(text), style);
        }

        private static string GetValueAsString(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue ?? "(null)";
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString("F2");
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Enum:
                    return property.enumDisplayNames[property.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return property.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return property.vector3Value.ToString();
                case SerializedPropertyType.Color:
                    return property.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null
                        ? property.objectReferenceValue.name
                        : "(None)";
                default:
                    return $"[{property.propertyType}]";
            }
        }
    }
}
