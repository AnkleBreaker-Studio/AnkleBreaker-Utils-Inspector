using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredDrawer : PropertyDrawer
    {
        private const float HelpBoxHeight = 24f;
        private const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isMissing = IsMissing(property);

            if (isMissing)
            {
                RequiredAttribute req = (RequiredAttribute)attribute;
                string message = string.IsNullOrEmpty(req.Message)
                    ? $"{property.displayName} is required."
                    : req.Message;

                Rect helpBoxRect = new Rect(position.x, position.y, position.width, HelpBoxHeight);
                EditorGUI.HelpBox(helpBoxRect, message, MessageType.Error);

                Rect propertyRect = new Rect(position.x, position.y + HelpBoxHeight + Spacing,
                    position.width, EditorGUI.GetPropertyHeight(property, label, true));
                EditorGUI.PropertyField(propertyRect, property, label, true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);

            if (IsMissing(property))
                return baseHeight + HelpBoxHeight + Spacing;

            return baseHeight;
        }

        private static bool IsMissing(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue == null;
                case SerializedPropertyType.String:
                    return string.IsNullOrEmpty(property.stringValue);
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue == null;
                default:
                    return false;
            }
        }
    }
}
