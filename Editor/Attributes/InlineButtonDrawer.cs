using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(InlineButtonAttribute))]
    public class InlineButtonDrawer : PropertyDrawer
    {
        private const float DefaultButtonWidth = 60f;
        private const float Spacing = 4f;
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (InlineButtonAttribute)attribute;

            string buttonLabel = attr.Label ?? ButtonDrawerUtility.FormatMethodName(attr.MethodName);
            float buttonWidth = attr.Width > 0 ? attr.Width : GUI.skin.button.CalcSize(new GUIContent(buttonLabel)).x + 8f;
            buttonWidth = Mathf.Max(buttonWidth, 24f);

            Rect fieldRect = new Rect(position.x, position.y, position.width - buttonWidth - Spacing, position.height);
            Rect buttonRect = new Rect(fieldRect.xMax + Spacing, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, property, label, true);

            bool isEnabled = IsButtonEnabled(attr.Mode);
            EditorGUI.BeginDisabledGroup(!isEnabled);

            if (GUI.Button(buttonRect, buttonLabel))
            {
                InvokeMethod(property, attr.MethodName);
            }

            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private static void InvokeMethod(SerializedProperty property, string methodName)
        {
            object target = property.serializedObject.targetObject;
            Type type = target.GetType();

            while (type != null)
            {
                MethodInfo method = type.GetMethod(methodName, MemberFlags);
                if (method != null)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, methodName);
                    method.Invoke(target, null);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                    property.serializedObject.Update();
                    return;
                }
                type = type.BaseType;
            }

            Debug.LogWarning($"[InlineButton] Method '{methodName}' not found on {target.GetType().Name}");
        }

        private static bool IsButtonEnabled(ButtonMode mode)
        {
            switch (mode)
            {
                case ButtonMode.EnabledInPlayMode: return Application.isPlaying;
                case ButtonMode.DisabledInPlayMode: return !Application.isPlaying;
                default: return true;
            }
        }
    }
}
