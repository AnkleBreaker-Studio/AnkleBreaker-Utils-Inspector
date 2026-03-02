using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public class LabelTextPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelTextAttribute labelTextAttribute = attribute as LabelTextAttribute;

            if (labelTextAttribute != null)
            {
                EditorGUI.PropertyField(
                    position, property, new GUIContent(labelTextAttribute.DisplayName), true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}