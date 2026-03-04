using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = IsEnabled(property);
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool IsEnabled(SerializedProperty property)
        {
            EnableIfAttribute attr = (EnableIfAttribute)attribute;

            if (attr.HasCompareValue)
                return ConditionResolver.EvaluateComparison(property, attr.ConditionName, attr.CompareValue);

            return ConditionResolver.Evaluate(property, attr.ConditionName);
        }
    }
}
