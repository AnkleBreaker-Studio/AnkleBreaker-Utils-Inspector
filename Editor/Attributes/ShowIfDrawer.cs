using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool shouldShow = ConditionResolver.Evaluate(property, ((ShowIfAttribute)attribute).ConditionName);

            if (shouldShow)
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool shouldShow = ConditionResolver.Evaluate(property, ((ShowIfAttribute)attribute).ConditionName);
            return shouldShow ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
        }
    }
}
