using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(MultilinePropertyAttribute))]
    public class MultilinePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MultilinePropertyAttribute attr = (MultilinePropertyAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            float textAreaHeight = EditorGUIUtility.singleLineHeight * attr.Lines;
            Rect textRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2f, position.y, position.width - EditorGUIUtility.labelWidth - 2f, textAreaHeight);

            EditorGUI.BeginChangeCheck();
            string newVal = EditorGUI.TextArea(textRect, property.stringValue);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = newVal;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MultilinePropertyAttribute attr = (MultilinePropertyAttribute)attribute;
            return EditorGUIUtility.singleLineHeight * attr.Lines;
        }
    }
}
