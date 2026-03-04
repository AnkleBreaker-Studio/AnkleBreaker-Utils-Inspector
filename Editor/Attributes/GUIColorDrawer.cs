using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(GUIColorAttribute))]
    public class GUIColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIColorAttribute attr = (GUIColorAttribute)attribute;

            Color prevColor = GUI.color;
            GUI.color = new Color(attr.R, attr.G, attr.B, attr.A);
            EditorGUI.PropertyField(position, property, label, true);
            GUI.color = prevColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
