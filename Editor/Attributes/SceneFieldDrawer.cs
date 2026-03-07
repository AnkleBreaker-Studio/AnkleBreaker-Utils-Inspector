using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SceneFieldAttribute))]
    public class SceneFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Find current scene asset from stored path
            SceneAsset currentScene = null;
            if (!string.IsNullOrEmpty(property.stringValue))
                currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            // Draw object field for scene asset
            float fieldHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width, fieldHeight);

            EditorGUI.BeginChangeCheck();
            var newScene = (SceneAsset)EditorGUI.ObjectField(fieldRect, label, currentScene, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newScene != null ? AssetDatabase.GetAssetPath(newScene) : "";
            }

            // Validate: is scene in Build Settings?
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                bool inBuildSettings = false;
                bool isEnabled = false;
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == property.stringValue)
                    {
                        inBuildSettings = true;
                        isEnabled = scene.enabled;
                        break;
                    }
                }

                Rect warningRect = new Rect(position.x, position.y + fieldHeight + 2f, position.width, fieldHeight);

                if (!inBuildSettings)
                    EditorGUI.HelpBox(warningRect, "Scene is NOT in Build Settings!", MessageType.Error);
                else if (!isEnabled)
                    EditorGUI.HelpBox(warningRect, "Scene is in Build Settings but DISABLED.", MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.propertyType == SerializedPropertyType.String && !string.IsNullOrEmpty(property.stringValue))
            {
                bool inBuildSettings = false;
                bool isEnabled = false;
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == property.stringValue)
                    {
                        inBuildSettings = true;
                        isEnabled = scene.enabled;
                        break;
                    }
                }

                if (!inBuildSettings || !isEnabled)
                    height += EditorGUIUtility.singleLineHeight + 2f;
            }

            return height;
        }
    }
}
