using System;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(EnumToggleButtonsAttribute))]
    public class EnumToggleButtonsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "[EnumToggleButtons] requires enum");
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            string[] names = property.enumDisplayNames;
            int selected = property.enumValueIndex;

            EditorGUI.BeginChangeCheck();
            int newSelected = GUI.Toolbar(position, selected, names);
            if (EditorGUI.EndChangeCheck())
                property.enumValueIndex = newSelected;
        }
    }
}
