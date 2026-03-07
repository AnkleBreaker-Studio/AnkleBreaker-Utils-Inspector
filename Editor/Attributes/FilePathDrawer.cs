using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathDrawer : PropertyDrawer
    {
        private bool _requestBrowse;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FilePathAttribute attr = (FilePathAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            float btnWidth = 25f;
            Rect fieldRect = new Rect(position.x, position.y, position.width - btnWidth - 2f, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.xMax - btnWidth, position.y, btnWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);
            property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder Icon")))
                _requestBrowse = true;

            EditorGUI.EndProperty();

            if (_requestBrowse)
            {
                _requestBrowse = false;

                string currentPath = property.stringValue;
                string startDir = string.IsNullOrEmpty(currentPath) ? "Assets" : System.IO.Path.GetDirectoryName(currentPath);
                if (string.IsNullOrEmpty(startDir)) startDir = "Assets";
                if (!System.IO.Path.IsPathRooted(startDir))
                    startDir = System.IO.Path.GetFullPath(startDir);

                string extensions = string.IsNullOrEmpty(attr.Extensions) ? "" : attr.Extensions;
                string selected = EditorUtility.OpenFilePanel("Select File", startDir, extensions);

                if (!string.IsNullOrEmpty(selected))
                {
                    if (!attr.AbsolutePath)
                    {
                        string projectRoot = System.IO.Path.GetFullPath(Application.dataPath + "/..").Replace("\\", "/");
                        selected = selected.Replace("\\", "/");
                        if (selected.StartsWith(projectRoot))
                            selected = selected.Substring(projectRoot.Length + 1);
                    }
                    property.stringValue = selected;
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUIUtility.ExitGUI();
            }
        }
    }
}
