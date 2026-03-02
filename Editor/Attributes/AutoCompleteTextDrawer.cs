using System;
using System.Reflection;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
#if ODIN_INSPECTOR
    public class AutoCompleteTextDrawer : OdinAttributeDrawer<AutoCompleteTextAttribute, string>
    {
        private static readonly AutoCompleteTextDrawer.ScrollableTextAreaInternalDelegate
            EditorGUI_ScrollableTextAreaInternal;

        private static readonly FieldInfo EditorGUI_s_TextAreaHash_Field;
        private static readonly int EditorGUI_s_TextAreaHash;
        private Vector2 scrollPosition;

        static AutoCompleteTextDrawer()
        {
            MethodInfo method = typeof(EditorGUI).GetMethod("ScrollableTextAreaInternal",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != (MethodInfo)null)
                AutoCompleteTextDrawer.EditorGUI_ScrollableTextAreaInternal =
                    (AutoCompleteTextDrawer.ScrollableTextAreaInternalDelegate)Delegate.CreateDelegate(
                        typeof(AutoCompleteTextDrawer.ScrollableTextAreaInternalDelegate), method);
            AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash_Field = typeof(EditorGUI).GetField("s_TextAreaHash",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (!(AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash_Field != (FieldInfo)null))
                return;
            AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash =
                (int)AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash_Field.GetValue((object)null);
        }

        /// <summary>
        /// Draws the property in the Rect provided. This method does not support the GUILayout, and is only called by DrawPropertyImplementation if the GUICallType is set to Rect, which is not the default.
        /// If the GUICallType is set to Rect, both GetRectHeight and DrawPropertyRect needs to be implemented.
        /// If the GUICallType is set to GUILayout, implementing DrawPropertyLayout will suffice.
        /// </summary>
        /// <param name="label">The label. This can be null, so make sure your drawer supports that.</param>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            IPropertyValueEntry<string> valueEntry = this.ValueEntry;
            AutoCompleteTextAttribute attribute = this.Attribute;
            float height = 32f +
                           (float)((Mathf.Clamp(
                               Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(
                                   GUIHelper.TempContent(valueEntry.SmartValue), GUIHelper.ContextWidth) / 13f),
                               attribute.MinLines, attribute.MaxLines) - 1) * 13);
            Rect controlRect = EditorGUILayout.GetControlRect(label != null, height);
            if (AutoCompleteTextDrawer.EditorGUI_ScrollableTextAreaInternal == null ||
                AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash_Field == (FieldInfo)null)
            {
                EditorGUI.LabelField(controlRect, label,
                    GUIHelper.TempContent("Cannot draw TextArea because Unity's internal API has changed."));
            }
            else
            {
                if (label != null)
                {
                    Rect rect = controlRect;
                    rect.height = 16f;
                    controlRect.yMin += rect.height;
                    GUIHelper.IndentRect(ref rect);
                    EditorGUI.HandlePrefixLabel(controlRect, rect, label);
                }

                if (Event.current.type == UnityEngine.EventType.Layout)
                    GUIUtility.GetControlID(AutoCompleteTextDrawer.EditorGUI_s_TextAreaHash, FocusType.Keyboard,
                        controlRect);

                string textTag = AutoCompleteText.InitFieldTag();

                valueEntry.SmartValue = AutoCompleteTextDrawer.EditorGUI_ScrollableTextAreaInternal(controlRect,
                    valueEntry.SmartValue, ref this.scrollPosition, EditorStyles.textArea);

                this.ValueEntry.SmartValue = AutoCompleteText.Process(controlRect, this.ValueEntry.SmartValue,
                    this.Attribute.Keys,
                    maxShownCount: 10, levenshteinDistance: 0.5f, this.Attribute.ToolTips,
                    textTag);
            }
        }

        private delegate string ScrollableTextAreaInternalDelegate(
            Rect position,
            string text,
            ref Vector2 scrollPosition,
            GUIStyle style);
    }
#else

     [CustomPropertyDrawer(typeof(AutoCompleteTextAttribute))]
    public class AutoCompleteTextDrawer : PropertyDrawer
    {
        private Vector2 scrollPosition;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AutoCompleteTextAttribute autoCompleteAttribute = (AutoCompleteTextAttribute)attribute;

            // Calculate height for the text area
            float height = 32f + (Mathf.Clamp(
                Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(new GUIContent(property.stringValue), position.width) /
                                13f),
                autoCompleteAttribute.MinLines, autoCompleteAttribute.MaxLines) - 1) * 13;

            // Draw the label
            if (label != null)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            // Draw the text area with scroll
            if (Event.current.type == EventType.Layout)
            {
                GUIUtility.GetControlID(FocusType.Keyboard, position);
            }

            Rect textAreaRect = new Rect(position.x, position.y, position.width, height);
            string newText = EditorGUI.TextArea(textAreaRect, property.stringValue, EditorStyles.textArea);

            // Update the property if text has changed
            if (newText != property.stringValue)
            {
                property.stringValue = newText;
            }

            // Process autocomplete
            string tag = AutoCompleteText.InitFieldTag();
            property.stringValue = AutoCompleteText.Process(textAreaRect, property.stringValue,
                autoCompleteAttribute.Keys,
                maxShownCount: 10, levenshteinDistance: 0.5f, autoCompleteAttribute.ToolTips, tag);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            AutoCompleteTextAttribute autoCompleteAttribute = (AutoCompleteTextAttribute)attribute;
            float height = 32f + (Mathf.Clamp(
                Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(new GUIContent(property.stringValue),
                    EditorGUIUtility.currentViewWidth) / 13f),
                autoCompleteAttribute.MinLines, autoCompleteAttribute.MaxLines) - 1) * 13;
            return height;
        }
    }

#endif
}