using AnkleBreaker.Utils.Inspector;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyEnumFlagsAttribute))]
    public class ReadOnlyEnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if UNITY_EDITOR
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                // Save the initial value
                int originalValue = property.intValue;

                // Save the current label color
                Color originalColor = GUI.color;

                // Darken the label color to indicate read-only mode
                GUI.color = Color.gray;

                // Show the interactive dropdown
                EditorGUI.BeginProperty(position, label, property);
                int newValue = EditorGUI.MaskField(position, label, originalValue, property.enumDisplayNames);
                EditorGUI.EndProperty();

                // Reapply the original value
                property.intValue = originalValue;

                GUI.color = originalColor;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use ReadOnlyEnumFlags with an Enum.");
            }
#endif
        }
    }
}