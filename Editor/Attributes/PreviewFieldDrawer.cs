using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(PreviewFieldAttribute))]
    public class PreviewFieldDrawer : PropertyDrawer
    {
        private const float Padding = 2f;
        private const float SelectBtnSize = 18f;

        private static GUIStyle _metaStyle;
        private static GUIStyle MetaStyle => _metaStyle ?? (_metaStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            wordWrap = true,
            richText = true,
            padding = new RectOffset(2, 2, 0, 0)
        });

        private static GUIStyle _pathStyle;
        private static GUIStyle PathStyle => _pathStyle ?? (_pathStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 9,
            wordWrap = true,
            richText = true,
            normal = { textColor = new Color(0.55f, 0.55f, 0.55f) }
        });

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PreviewFieldAttribute attr = (PreviewFieldAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "[PreviewField] requires Object reference");
                return;
            }

            Object obj = property.objectReferenceValue;

            // ---- EMPTY: standard object field ----
            if (obj == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // ---- ASSIGNED: custom layout ----
            float previewSize = attr.PreviewHeight;
            float totalHeight = previewSize;
            float lineH = EditorGUIUtility.singleLineHeight;

            // Preview rect on the RIGHT
            Rect previewRect = new Rect(
                position.xMax - previewSize,
                position.y,
                previewSize,
                previewSize);

            // Left column for label + details
            float leftWidth = position.width - previewSize - 6f;
            Rect leftArea = new Rect(position.x, position.y, leftWidth, totalHeight);

            // -- Draw label (variable name) --
            Rect labelRect = new Rect(leftArea.x, leftArea.y, leftArea.width, lineH);
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);

            // -- Draw metadata lines --
            float metaY = leftArea.y + lineH + Padding;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            DrawMetadataLines(leftArea.x, metaY, leftArea.width, obj, assetPath);

            // -- Draw preview background --
            EditorGUI.DrawRect(previewRect, new Color(0.15f, 0.15f, 0.15f, 1f));

            // -- Draw preview texture --
            Texture2D preview = AssetPreview.GetAssetPreview(obj);
            if (preview == null)
                preview = AssetPreview.GetMiniThumbnail(obj);
            if (preview != null)
                GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);

            // -- Make preview a drop target for drag & drop --
            HandlePreviewDragDrop(previewRect, property);

            // -- Button rects --
            Rect clearRect = new Rect(
                previewRect.xMax - SelectBtnSize - 2f,
                previewRect.y + 2f,
                SelectBtnSize, SelectBtnSize);

            Rect pingRect = new Rect(
                previewRect.xMax - SelectBtnSize - 2f,
                previewRect.yMax - SelectBtnSize - 2f,
                SelectBtnSize, SelectBtnSize);

            // -- Handle ALL clicks in one place (buttons have priority over preview) --
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                && previewRect.Contains(Event.current.mousePosition))
            {
                if (clearRect.Contains(Event.current.mousePosition))
                {
                    // Clear reference
                    property.objectReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else if (pingRect.Contains(Event.current.mousePosition))
                {
                    // Ping asset in project without changing selection
                    EditorGUIUtility.PingObject(obj);
                }
                else
                {
                    // Open native typed object picker via reflection
                    int id = GUIUtility.GetControlID(FocusType.Passive);
                    System.Type fieldType = fieldInfo != null ? fieldInfo.FieldType : typeof(Object);
                    OpenTypedObjectPicker(obj, fieldType, id);
                    _pickerPropertyPath = property.propertyPath;
                    _pickerSerializedObject = property.serializedObject;
                }
                Event.current.Use();
            }

            // Handle object picker result
            if (Event.current.commandName == "ObjectSelectorUpdated"
                && _pickerSerializedObject == property.serializedObject
                && _pickerPropertyPath == property.propertyPath)
            {
                Object picked = EditorGUIUtility.GetObjectPickerObject();
                if (picked != null)
                {
                    property.objectReferenceValue = picked;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            // -- Draw clear button (X) top-right --
            GUI.Button(clearRect, new GUIContent("\u2715"), EditorStyles.miniButton);

            // -- Draw ping button bottom-right --
            GUI.Button(pingRect, EditorGUIUtility.IconContent("d_Search Icon"), GUIStyle.none);
        }

        private static string _pickerPropertyPath;
        private static SerializedObject _pickerSerializedObject;

        private static void OpenTypedObjectPicker(Object current, System.Type type, int controlId)
        {
            // Use reflection to call ShowObjectPicker<T> with the correct runtime type
            // This opens the native Unity picker (Select Sprite, Select Texture, etc.)
            MethodInfo method = typeof(EditorGUIUtility)
                .GetMethod("ShowObjectPicker", BindingFlags.Static | BindingFlags.Public);
            if (method != null)
            {
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(null, new object[] { current, false, "", controlId });
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference
                || property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            PreviewFieldAttribute attr = (PreviewFieldAttribute)attribute;
            return attr.PreviewHeight;
        }

        private static void DrawMetadataLines(float x, float y, float width, Object obj, string assetPath)
        {
            float lineH = 13f;
            float curY = y;

            // Type-specific metadata
            if (obj is Texture2D tex)
            {
                EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                    tex.width + " x " + tex.height + " px", MetaStyle);
                curY += lineH;
            }
            else if (obj is Sprite spr)
            {
                int sw = Mathf.RoundToInt(spr.rect.width);
                int sh = Mathf.RoundToInt(spr.rect.height);
                EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                    sw + " x " + sh + " px", MetaStyle);
                curY += lineH;
            }
            else if (obj is Mesh mesh)
            {
                EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                    mesh.vertexCount.ToString("N0") + " verts", MetaStyle);
                curY += lineH;
                EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                    (mesh.triangles.Length / 3).ToString("N0") + " tris", MetaStyle);
                curY += lineH;
                Vector3 size = mesh.bounds.size;
                EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                    size.x.ToString("F2") + " x " + size.y.ToString("F2") + " x " + size.z.ToString("F2"), MetaStyle);
                curY += lineH;
            }
            else if (obj is GameObject go)
            {
                MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    Mesh goMesh = mf.sharedMesh;
                    EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                        goMesh.vertexCount.ToString("N0") + " verts, " + (goMesh.triangles.Length / 3).ToString("N0") + " tris", MetaStyle);
                    curY += lineH;
                }
            }

            // File size
            if (!string.IsNullOrEmpty(assetPath))
            {
                string fullPath = Path.GetFullPath(assetPath);
                if (File.Exists(fullPath))
                {
                    long bytes = new FileInfo(fullPath).Length;
                    EditorGUI.LabelField(new Rect(x, curY, width, lineH),
                        FormatFileSize(bytes), MetaStyle);
                    curY += lineH;
                }

                // Path
                EditorGUI.LabelField(new Rect(x, curY, width, lineH * 2f),
                    assetPath, PathStyle);
            }
        }

        private static void HandlePreviewDragDrop(Rect previewRect, SerializedProperty property)
        {
            Event evt = Event.current;
            if (!previewRect.Contains(evt.mousePosition)) return;

            if (evt.type == EventType.DragUpdated)
            {
                if (DragAndDrop.objectReferences.Length > 0)
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                evt.Use();
            }
            else if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    property.objectReferenceValue = DragAndDrop.objectReferences[0];
                    property.serializedObject.ApplyModifiedProperties();
                }
                evt.Use();
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024f).ToString("F1") + " KB";
            return (bytes / (1024f * 1024f)).ToString("F2") + " MB";
        }
    }
}
