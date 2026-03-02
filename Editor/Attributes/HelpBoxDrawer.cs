using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HelpBoxAttribute helpBox = (HelpBoxAttribute)attribute;
            float messageHeight = GetMessageHeight(helpBox);
            return messageHeight + EditorGUIUtility.singleLineHeight * 1.5f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HelpBoxAttribute helpBox = (HelpBoxAttribute)attribute;

            float messageHeight = GetMessageHeight(helpBox);
            
            Rect helpBoxRect = new Rect(position.x, position.y, position.width, messageHeight);
            EditorGUI.HelpBox(helpBoxRect, helpBox.Message, (MessageType)helpBox.Type);

            bool isMultiLine = IsMultiLine(helpBox.Message);

            if (isMultiLine)
            {
                position.y += messageHeight + EditorGUIUtility.singleLineHeight * 0.5f;
            }
            else
            {
                position.y += EditorGUIUtility.singleLineHeight * 2;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        private static float GetMessageHeight(HelpBoxAttribute helpBox)
        {
            return EditorStyles.helpBox.CalcHeight(new GUIContent(helpBox.Message), EditorGUIUtility.currentViewWidth);
        }

        private static bool IsMultiLine(string message)
        {
            float singleLineHeight = EditorStyles.helpBox.lineHeight;
            float messageHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth);
            return messageHeight > singleLineHeight;
        }
    }
}