using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnValueChangedAttribute attr = (OnValueChangedAttribute)attribute;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();

                object target = property.serializedObject.targetObject;
                MethodInfo method = target.GetType().GetMethod(attr.MethodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method != null)
                    method.Invoke(target, null);
                else
                    Debug.LogWarning($"[OnValueChanged] Method not found: {attr.MethodName}");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
