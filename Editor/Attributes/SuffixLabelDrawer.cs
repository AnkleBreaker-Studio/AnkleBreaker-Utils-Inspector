using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SuffixLabelAttribute))]
    public class SuffixLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SuffixLabelAttribute attr = (SuffixLabelAttribute)attribute;

            float suffixWidth = EditorStyles.miniLabel.CalcSize(new GUIContent(attr.Suffix)).x + 4f;
            Rect fieldRect = new Rect(position.x, position.y, position.width - suffixWidth, position.height);
            Rect suffixRect = new Rect(position.xMax - suffixWidth, position.y, suffixWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);

            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) }
            };
            EditorGUI.LabelField(suffixRect, attr.Suffix, style);
        }
    }
}
