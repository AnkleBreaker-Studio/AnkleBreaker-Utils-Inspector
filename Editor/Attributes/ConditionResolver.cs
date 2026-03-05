using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Shared utility for resolving conditions by name.
    /// Used by HideIfDrawer, ShowIfDrawer, and EnableIfDrawer to evaluate fields, properties, or methods.
    /// Supports both boolean conditions and value comparisons (enum, int, string).
    /// </summary>
    internal static class ConditionResolver
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static FieldInfo FindField(Type type, string name)
        {
            while (type != null)
            {
                FieldInfo f = type.GetField(name, Flags);
                if (f != null) return f;
                type = type.BaseType;
            }
            return null;
        }

        private static PropertyInfo FindProperty(Type type, string name)
        {
            while (type != null)
            {
                PropertyInfo p = type.GetProperty(name, Flags);
                if (p != null) return p;
                type = type.BaseType;
            }
            return null;
        }

        private static MethodInfo FindMethod(Type type, string name)
        {
            while (type != null)
            {
                MethodInfo m = type.GetMethod(name, Flags);
                if (m != null) return m;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Evaluates a named boolean condition on the target object of a SerializedProperty.
        /// Looks for a field, property, or parameterless method returning bool.
        /// Walks the full type hierarchy (base classes included).
        /// </summary>
        public static bool Evaluate(SerializedProperty property, string conditionName, bool fallback = false)
        {
            object targetObject = property.serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            FieldInfo field = FindField(targetType, conditionName);
            if (field != null && field.FieldType == typeof(bool))
                return (bool)field.GetValue(targetObject);

            PropertyInfo prop = FindProperty(targetType, conditionName);
            if (prop != null && prop.PropertyType == typeof(bool))
                return (bool)prop.GetValue(targetObject);

            MethodInfo method = FindMethod(targetType, conditionName);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
                return (bool)method.Invoke(targetObject, null);

            Debug.LogWarning(
                $"[ConditionResolver] No matching boolean field, property, or method found for condition: {conditionName}");
            return fallback;
        }

        /// <summary>
        /// Evaluates whether a named field/property equals the given compare value.
        /// Supports enums (compared as int), ints, and strings.
        /// Walks the full type hierarchy (base classes included).
        /// </summary>
        public static bool EvaluateComparison(SerializedProperty property, string fieldName, object compareValue, bool fallback = false)
        {
            object targetObject = property.serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            // Try field
            FieldInfo field = FindField(targetType, fieldName);
            if (field != null)
                return CompareValues(field.GetValue(targetObject), compareValue);

            // Try property
            PropertyInfo prop = FindProperty(targetType, fieldName);
            if (prop != null)
                return CompareValues(prop.GetValue(targetObject), compareValue);

            Debug.LogWarning(
                $"[ConditionResolver] No matching field or property found for comparison: {fieldName}");
            return fallback;
        }

        private static bool CompareValues(object fieldValue, object compareValue)
        {
            if (fieldValue == null && compareValue == null) return true;
            if (fieldValue == null || compareValue == null) return false;

            // Enum comparison: compare as int
            if (fieldValue.GetType().IsEnum && compareValue is int intVal)
                return Convert.ToInt32(fieldValue) == intVal;

            // Int comparison
            if (fieldValue is int fieldInt && compareValue is int cmpInt)
                return fieldInt == cmpInt;

            // String comparison
            if (fieldValue is string fieldStr && compareValue is string cmpStr)
                return string.Equals(fieldStr, cmpStr, StringComparison.Ordinal);

            // Fallback: generic Equals
            return fieldValue.Equals(compareValue);
        }
    }
}
