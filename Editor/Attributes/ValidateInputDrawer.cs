using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ValidateInputAttribute))]
    public class ValidateInputDrawer : PropertyDrawer
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidateInputAttribute attr = (ValidateInputAttribute)attribute;
            bool isValid = Validate(property, attr.ValidatorMethod);

            if (!isValid)
            {
                float helpHeight = EditorGUIUtility.singleLineHeight * 1.5f;
                Rect helpRect = new Rect(position.x, position.y, position.width, helpHeight);
                EditorGUI.HelpBox(helpRect, attr.Message, (MessageType)attr.MessageType);
                position.y += helpHeight + 2f;
                position.height -= helpHeight + 2f;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ValidateInputAttribute attr = (ValidateInputAttribute)attribute;
            float height = EditorGUI.GetPropertyHeight(property, label, true);

            if (!Validate(property, attr.ValidatorMethod))
                height += EditorGUIUtility.singleLineHeight * 1.5f + 2f;

            return height;
        }

        private bool Validate(SerializedProperty property, string methodName)
        {
            object target = property.serializedObject.targetObject;
            Type type = target.GetType();

            MethodInfo method = type.GetMethod(methodName, Flags);
            if (method == null || method.ReturnType != typeof(bool))
                return true;

            var parameters = method.GetParameters();
            if (parameters.Length == 0)
                return (bool)method.Invoke(target, null);

            return true;
        }
    }
}
