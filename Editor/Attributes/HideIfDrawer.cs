using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldHide(property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldHide(property) ? 0 : EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool ShouldHide(SerializedProperty property)
        {
            HideIfAttribute attr = (HideIfAttribute)attribute;

            if (attr.HasCompareValue)
                return ConditionResolver.EvaluateComparison(property, attr.ConditionName, attr.CompareValue);

            return ConditionResolver.Evaluate(property, attr.ConditionName);
        }
    }
}
