using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ListDrawerSettingsAttribute))]
    public class ListDrawerSettingsDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> _listCache = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            ListDrawerSettingsAttribute attr = (ListDrawerSettingsAttribute)attribute;
            var list = GetList(property, label, attr);

            // Enforce min/max count
            EnforceConstraints(property, attr);

            list.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)
                return EditorGUIUtility.singleLineHeight;

            ListDrawerSettingsAttribute attr = (ListDrawerSettingsAttribute)attribute;
            var list = GetList(property, label, attr);
            return list.GetHeight();
        }

        private ReorderableList GetList(SerializedProperty property, GUIContent label, ListDrawerSettingsAttribute attr)
        {
            string key = property.propertyPath;
            if (_listCache.TryGetValue(key, out var cached) && cached.serializedProperty.serializedObject == property.serializedObject)
                return cached;

            string headerText = label.text;
            if (attr.ShowCount)
                headerText += $" [{property.arraySize}]";

            var list = new ReorderableList(property.serializedObject, property,
                attr.Draggable, true, attr.ShowAddRemoveButtons, attr.ShowAddRemoveButtons);

            list.drawHeaderCallback = rect =>
            {
                string h = label.text;
                if (attr.ShowCount) h += $" [{property.arraySize}]";
                EditorGUI.LabelField(rect, h, EditorStyles.boldLabel);
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                rect.y += 2f;
                rect.height = EditorGUI.GetPropertyHeight(element, true);

                string elementLabel;
                if (!string.IsNullOrEmpty(attr.ElementLabel))
                    elementLabel = attr.ElementLabel.Replace("$index", index.ToString());
                else
                    elementLabel = $"Element {index}";

                EditorGUI.PropertyField(rect, element, new GUIContent(elementLabel), true);
            };

            list.elementHeightCallback = index =>
            {
                if (index >= property.arraySize) return EditorGUIUtility.singleLineHeight;
                var element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4f;
            };

            // Enforce max count on add
            if (attr.MaxCount > 0)
            {
                list.onCanAddCallback = l => l.count < attr.MaxCount;
            }

            // Enforce min count on remove
            if (attr.MinCount > 0)
            {
                list.onCanRemoveCallback = l => l.count > attr.MinCount;
            }

            _listCache[key] = list;
            return list;
        }

        private static void EnforceConstraints(SerializedProperty property, ListDrawerSettingsAttribute attr)
        {
            if (attr.MinCount > 0 && property.arraySize < attr.MinCount)
            {
                while (property.arraySize < attr.MinCount)
                    property.InsertArrayElementAtIndex(property.arraySize);
            }
        }
    }
}
