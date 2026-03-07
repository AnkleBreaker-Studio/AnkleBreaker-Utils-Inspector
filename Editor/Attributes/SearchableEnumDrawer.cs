using System;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "[SearchableEnum] requires enum");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect = EditorGUI.PrefixLabel(position, label);
            string currentName = property.enumDisplayNames[property.enumValueIndex];

            if (EditorGUI.DropdownButton(fieldRect, new GUIContent(currentName), FocusType.Keyboard))
            {
                SearchableEnumPopup.Show(fieldRect, property.enumDisplayNames, property.enumValueIndex, index =>
                {
                    property.enumValueIndex = index;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            EditorGUI.EndProperty();
        }
    }

    public class SearchableEnumPopup : EditorWindow
    {
        private string[] _names;
        private int _selectedIndex;
        private Action<int> _onSelect;
        private string _searchText = "";
        private Vector2 _scroll;

        public static void Show(Rect buttonRect, string[] names, int current, Action<int> onSelect)
        {
            var window = CreateInstance<SearchableEnumPopup>();
            window._names = names;
            window._selectedIndex = current;
            window._onSelect = onSelect;

            Vector2 size = new Vector2(Mathf.Max(buttonRect.width, 200f), 300f);
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            window.ShowAsDropDown(buttonRect, size);
            window.Focus();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);

            GUI.SetNextControlName("SearchField");
            _searchText = EditorGUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);
            EditorGUI.FocusTextInControl("SearchField");

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            string search = _searchText.ToLowerInvariant();
            for (int i = 0; i < _names.Length; i++)
            {
                if (!string.IsNullOrEmpty(search) && !_names[i].ToLowerInvariant().Contains(search))
                    continue;

                bool isSelected = i == _selectedIndex;
                GUIStyle style = isSelected ? "SelectionRect" : "MenuItem";

                if (GUILayout.Button(_names[i], style, GUILayout.Height(20f)))
                {
                    _onSelect?.Invoke(i);
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                Close();
        }
    }
}
