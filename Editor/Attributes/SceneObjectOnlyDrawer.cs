using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SceneObjectOnlyAttribute))]
    public class SceneObjectOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            Object newObj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, fieldInfo.FieldType, true);
            if (EditorGUI.EndChangeCheck())
            {
                if (newObj == null || !EditorUtility.IsPersistent(newObj))
                    property.objectReferenceValue = newObj;
                else
                    Debug.LogWarning($"[SceneObjectOnly] Rejected asset '{newObj.name}'. Only scene objects are allowed.");
            }

            EditorGUI.EndProperty();
        }
    }
}
