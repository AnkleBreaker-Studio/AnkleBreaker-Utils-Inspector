using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FolderPathAttribute folderPathAttribute = (FolderPathAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.HelpBox(position, "FolderPath attribute can only be used with string type.",
                    MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects
            Rect textFieldRect = new Rect(position.x, position.y, position.width - 25, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 25, position.y, 25, position.height);

            // Set the default path if the string is empty
            if (string.IsNullOrEmpty(property.stringValue) && !string.IsNullOrEmpty(folderPathAttribute.DefaultPath))
            {
                property.stringValue = folderPathAttribute.DefaultPath;
            }

            // Draw the text field
            property.stringValue = EditorGUI.TextField(textFieldRect, label, property.stringValue);

            // Draw the ping button
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Folder Icon")))
            {
                string path = property.stringValue;
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                }
                else
                {
                    Debug.LogWarning("Folder path is invalid or not found: " + path);
                }
            }

            // Handle drag and drop
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
            {
                if (position.Contains(currentEvent.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(draggedObject);
                            if (AssetDatabase.IsValidFolder(path))
                            {
                                property.stringValue = path;
                                GUI.changed = true;
                            }
                        }
                    }

                    currentEvent.Use();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}