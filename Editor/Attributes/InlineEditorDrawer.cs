using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer : PropertyDrawer
    {
        private UnityEditor.Editor _cachedEditor;
        private bool _foldout = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "[InlineEditor] requires Object reference");
                return;
            }

            InlineEditorAttribute attr = (InlineEditorAttribute)attribute;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            Object obj = property.objectReferenceValue;
            if (obj == null) return;

            _foldout = EditorGUI.Foldout(
                new Rect(position.x - 12f, position.y, 12f, lineHeight),
                _foldout, GUIContent.none);

            if (!_foldout) return;

            if (_cachedEditor == null || _cachedEditor.target != obj)
            {
                if (_cachedEditor != null)
                    Object.DestroyImmediate(_cachedEditor);
                UnityEditor.Editor.CreateCachedEditor(obj, null, ref _cachedEditor);
            }

            if (_cachedEditor != null)
            {
                Rect inlineRect = new Rect(position.x + 16f, position.y + lineHeight + 2f,
                    position.width - 16f, position.height - lineHeight - 2f);

                EditorGUI.indentLevel++;
                Rect boxRect = new Rect(position.x + 12f, position.y + lineHeight,
                    position.width - 12f, position.height - lineHeight);
                GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

                GUILayout.BeginArea(inlineRect);
                _cachedEditor.OnInspectorGUI();
                GUILayout.EndArea();
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue != null && _foldout)
            {
                height += 120f; // Approximate inline editor height
            }

            return height;
        }
    }
}
