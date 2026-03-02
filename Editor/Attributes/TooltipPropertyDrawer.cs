#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ABToolTipAttribute))]
    public class ABTooltipAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ABToolTipAttribute abToolTipAttribute = attribute as ABToolTipAttribute;

            if (abToolTipAttribute != null)
            {
                string tooltipMemberName = abToolTipAttribute.TooltipMemberName;
                
                SerializedProperty tooltipField = property.serializedObject.FindProperty(tooltipMemberName);

                if (tooltipField != null && tooltipField.propertyType == SerializedPropertyType.String)
                {
                    label.tooltip = tooltipField.stringValue;
                }
                else
                {
                    System.Reflection.FieldInfo privateField = property.serializedObject.targetObject.GetType().GetField(tooltipMemberName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (privateField != null && privateField.FieldType == typeof(string))
                    {
                        string tooltip = (string)privateField.GetValue(property.serializedObject.targetObject);
                        label.tooltip = tooltip;
                    }
                    else
                    {
                        System.Reflection.MethodInfo methodInfo = property.serializedObject.targetObject.GetType().GetMethod(tooltipMemberName);

                        if (methodInfo != null)
                        {
                            string tooltip = (string)methodInfo.Invoke(property.serializedObject.targetObject, null);
                            label.tooltip = tooltip;
                        }
                    }
                }
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif
