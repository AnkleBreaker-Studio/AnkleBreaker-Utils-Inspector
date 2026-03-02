using UnityEngine;
using UnityEditor;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HideVariableNameAttribute))]
    public class HideVariableNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }
}
