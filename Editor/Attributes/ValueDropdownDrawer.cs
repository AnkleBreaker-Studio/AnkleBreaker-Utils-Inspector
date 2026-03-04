using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ValueDropdownAttribute))]
    public class ValueDropdownDrawer : PropertyDrawer
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValueDropdownAttribute attr = (ValueDropdownAttribute)attribute;
            object target = property.serializedObject.targetObject;
            IList values = GetValues(target, attr.MemberName);

            if (values == null || values.Count == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string[] options = new string[values.Count];
            int selectedIndex = 0;

            for (int i = 0; i < values.Count; i++)
            {
                options[i] = values[i] != null ? values[i].ToString() : "(null)";

                if (property.propertyType == SerializedPropertyType.String && property.stringValue == options[i])
                    selectedIndex = i;
                else if (property.propertyType == SerializedPropertyType.Integer && values[i] is int intVal && property.intValue == intVal)
                    selectedIndex = i;
                else if (property.propertyType == SerializedPropertyType.Float && values[i] is float fVal && Mathf.Approximately(property.floatValue, fVal))
                    selectedIndex = i;
            }

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);
            if (EditorGUI.EndChangeCheck() && newIndex < values.Count)
            {
                object val = values[newIndex];
                if (property.propertyType == SerializedPropertyType.String)
                    property.stringValue = val?.ToString() ?? "";
                else if (property.propertyType == SerializedPropertyType.Integer && val is int iv)
                    property.intValue = iv;
                else if (property.propertyType == SerializedPropertyType.Float && val is float fv)
                    property.floatValue = fv;
            }
        }

        private IList GetValues(object target, string memberName)
        {
            Type type = target.GetType();

            FieldInfo field = type.GetField(memberName, Flags);
            if (field != null && typeof(IList).IsAssignableFrom(field.FieldType))
                return (IList)field.GetValue(target);

            PropertyInfo prop = type.GetProperty(memberName, Flags);
            if (prop != null && typeof(IList).IsAssignableFrom(prop.PropertyType))
                return (IList)prop.GetValue(target);

            MethodInfo method = type.GetMethod(memberName, Flags);
            if (method != null && typeof(IList).IsAssignableFrom(method.ReturnType) && method.GetParameters().Length == 0)
                return (IList)method.Invoke(target, null);

            return null;
        }
    }
}
