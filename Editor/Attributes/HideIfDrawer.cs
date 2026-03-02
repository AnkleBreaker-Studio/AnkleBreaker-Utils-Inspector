using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIf = (HideIfAttribute)attribute;
            bool shouldHide = ShouldHide(property, hideIf.ConditionName);

            if (!shouldHide)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIf = (HideIfAttribute)attribute;
            bool shouldHide = ShouldHide(property, hideIf.ConditionName);

            if (shouldHide)
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool ShouldHide(SerializedProperty property, string conditionName)
        {
            SerializedObject serializedObject = property.serializedObject;
            UnityEngine.Object targetObject = serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            // Check for boolean field or property
            FieldInfo field = targetType.GetField(conditionName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && field.FieldType == typeof(bool))
            {
                return (bool)field.GetValue(targetObject);
            }

            PropertyInfo prop = targetType.GetProperty(conditionName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                return (bool)prop.GetValue(targetObject);
            }

            // Check for method returning boolean
            MethodInfo method = targetType.GetMethod(conditionName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
            {
                bool value = (bool)method.Invoke(targetObject, null);
                return value;
            }

            Debug.LogWarning(
                $"No matching boolean field, property, or method found for HideIf condition: {conditionName}");
            return false;
        }
    }
}