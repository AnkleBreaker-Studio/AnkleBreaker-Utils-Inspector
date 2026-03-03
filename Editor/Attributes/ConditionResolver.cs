using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Shared utility for resolving boolean conditions by name.
    /// Used by HideIfDrawer and ShowIfDrawer to evaluate fields, properties, or methods.
    /// </summary>
    internal static class ConditionResolver
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// Evaluates a named boolean condition on the target object of a SerializedProperty.
        /// Looks for a field, property, or parameterless method returning bool.
        /// </summary>
        /// <param name="property">The serialized property whose target object owns the condition.</param>
        /// <param name="conditionName">Name of the field, property, or method to evaluate.</param>
        /// <param name="fallback">Value returned if the condition cannot be found.</param>
        /// <returns>The boolean result of the condition, or <paramref name="fallback"/> if not found.</returns>
        public static bool Evaluate(SerializedProperty property, string conditionName, bool fallback = false)
        {
            Object targetObject = property.serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            FieldInfo field = targetType.GetField(conditionName, Flags);
            if (field != null && field.FieldType == typeof(bool))
                return (bool)field.GetValue(targetObject);

            PropertyInfo prop = targetType.GetProperty(conditionName, Flags);
            if (prop != null && prop.PropertyType == typeof(bool))
                return (bool)prop.GetValue(targetObject);

            MethodInfo method = targetType.GetMethod(conditionName, Flags);
            if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
                return (bool)method.Invoke(targetObject, null);

            Debug.LogWarning(
                $"No matching boolean field, property, or method found for condition: {conditionName}");
            return fallback;
        }
    }
}
