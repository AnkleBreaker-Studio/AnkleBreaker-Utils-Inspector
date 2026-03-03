using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool shouldHide = ConditionResolver.Evaluate(property, ((HideIfAttribute)attribute).ConditionName);

            if (!shouldHide)
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool shouldHide = ConditionResolver.Evaluate(property, ((HideIfAttribute)attribute).ConditionName);
            return shouldHide ? 0 : EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
