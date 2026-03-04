using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(PreviewFieldAttribute))]
    public class PreviewFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PreviewFieldAttribute attr = (PreviewFieldAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "[PreviewField] requires Object reference");
                return;
            }

            float fieldHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width, fieldHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            if (property.objectReferenceValue != null)
            {
                Texture2D preview = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                if (preview == null)
                    preview = AssetPreview.GetMiniThumbnail(property.objectReferenceValue);

                if (preview != null)
                {
                    float previewSize = attr.PreviewHeight;
                    Rect previewRect = new Rect(
                        position.x + EditorGUIUtility.labelWidth,
                        position.y + fieldHeight + 2f,
                        previewSize,
                        previewSize);

                    GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PreviewFieldAttribute attr = (PreviewFieldAttribute)attribute;
            float height = EditorGUIUtility.singleLineHeight;

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                height += attr.PreviewHeight + 4f;

            return height;
        }
    }
}
