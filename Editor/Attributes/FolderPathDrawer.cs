using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private bool _requestBrowse;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FolderPathAttribute attr = (FolderPathAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Set default path
            if (string.IsNullOrEmpty(property.stringValue) && !string.IsNullOrEmpty(attr.DefaultPath))
                property.stringValue = attr.DefaultPath;

            float btnWidth = 25f;
            float validationHeight = 0f;
            bool pathInvalid = false;

            // Check if path is invalid when RequireExistingPath is set
            if (attr.RequireExistingPath && !string.IsNullOrEmpty(property.stringValue))
            {
                string checkPath = property.stringValue;
                if (!attr.AbsolutePath)
                    checkPath = System.IO.Path.GetFullPath(checkPath);
                pathInvalid = !System.IO.Directory.Exists(checkPath);
                if (pathInvalid) validationHeight = EditorGUIUtility.singleLineHeight + 2f;
            }

            // Layout rects
            Rect fieldRect = new Rect(position.x, position.y, position.width - btnWidth - 2f, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.xMax - btnWidth, position.y, btnWidth, EditorGUIUtility.singleLineHeight);

            // Draw property inside BeginProperty/EndProperty
            EditorGUI.BeginProperty(position, label, property);
            property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);

            // Browse button — only sets a flag, does NOT open the modal here
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder Icon")))
                _requestBrowse = true;

            // Drag and drop
            HandleDragDrop(position, property);

            // Validation warning
            if (pathInvalid)
            {
                Rect helpRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2f,
                    position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(helpRect, "Path does not exist: " + property.stringValue, MessageType.Error);
            }

            EditorGUI.EndProperty();

            // Open folder panel AFTER EndProperty to avoid stack corruption
            if (_requestBrowse)
            {
                _requestBrowse = false;

                string currentPath = property.stringValue;
                string startFolder = string.IsNullOrEmpty(currentPath) ? "Assets" : currentPath;

                // Resolve to full path for the panel
                if (!System.IO.Path.IsPathRooted(startFolder))
                    startFolder = System.IO.Path.GetFullPath(startFolder);

                string selected = EditorUtility.OpenFolderPanel("Select Folder", startFolder, "");
                if (!string.IsNullOrEmpty(selected))
                {
                    if (!attr.AbsolutePath)
                    {
                        // Convert to project-relative path
                        string projectRoot = System.IO.Path.GetFullPath(Application.dataPath + "/..");
                        selected = selected.Replace("\\", "/");
                        projectRoot = projectRoot.Replace("\\", "/");
                        if (selected.StartsWith(projectRoot))
                            selected = selected.Substring(projectRoot.Length + 1);
                    }
                    property.stringValue = selected;
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUIUtility.ExitGUI();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            FolderPathAttribute attr = (FolderPathAttribute)attribute;

            if (attr.RequireExistingPath && !string.IsNullOrEmpty(property.stringValue))
            {
                string checkPath = property.stringValue;
                if (!attr.AbsolutePath)
                    checkPath = System.IO.Path.GetFullPath(checkPath);
                if (!System.IO.Directory.Exists(checkPath))
                    height += EditorGUIUtility.singleLineHeight + 2f;
            }

            return height;
        }

        private static void HandleDragDrop(Rect position, SerializedProperty property)
        {
            Event evt = Event.current;
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform) return;
            if (!position.Contains(evt.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        property.stringValue = path;
                        GUI.changed = true;
                        break;
                    }
                }
            }

            evt.Use();
        }
    }
}