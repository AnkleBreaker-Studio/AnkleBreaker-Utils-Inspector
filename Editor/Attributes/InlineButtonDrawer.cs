using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(InlineButtonAttribute))]
    public class InlineButtonDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 60f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InlineButtonAttribute attr = (InlineButtonAttribute)attribute;

            Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - 4f, position.height);
            Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, label, true);

            string btnLabel = string.IsNullOrEmpty(attr.Label) ? attr.MethodName : attr.Label;
            if (GUI.Button(buttonRect, btnLabel))
            {
                object target = property.serializedObject.targetObject;
                MethodInfo method = target.GetType().GetMethod(attr.MethodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method != null)
                {
                    method.Invoke(target, null);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
                else
                {
                    Debug.LogWarning($"[InlineButton] Method not found: {attr.MethodName}");
                }
            }
        }
    }
}
