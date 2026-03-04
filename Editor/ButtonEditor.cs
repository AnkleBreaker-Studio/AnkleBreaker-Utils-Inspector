#if !ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Custom editor for all MonoBehaviours / ScriptableObjects (when Odin is not present).
    /// Handles: [Button], [BoxGroup], [FoldoutGroup], [TabGroup], [HorizontalGroup],
    /// [PropertyOrder], and [ShowInInspector].
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class ButtonMonoBehaviourEditor : ABGroupedEditor { }

    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ButtonScriptableObjectEditor : ABGroupedEditor { }

    /// <summary>
    /// Base editor that draws properties with group support and buttons.
    /// </summary>
    public class ABGroupedEditor : UnityEditor.Editor
    {
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private List<ButtonMethodInfo> _buttonMethods;
        private List<ShowInInspectorEntry> _showInInspectorEntries;
        private static readonly Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();
        private static readonly Dictionary<string, int> TabStates = new Dictionary<string, int>();

        private void OnEnable()
        {
            _buttonMethods = ButtonDrawerUtility.CollectButtonMethods(target);
            _showInInspectorEntries = CollectShowInInspector(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Collect all properties
            var entries = CollectPropertyEntries();

            // Draw ungrouped and grouped properties
            DrawPropertyEntries(entries);

            serializedObject.ApplyModifiedProperties();

            // ShowInInspector fields
            DrawShowInInspector();

            // Buttons
            ButtonDrawerUtility.DrawButtons(_buttonMethods, targets);
        }

        #region Property Collection

        private struct PropertyEntry
        {
            public SerializedProperty Property;
            public string BoxGroup;
            public string FoldoutGroup;
            public string TabGroup;
            public string HorizontalGroup;
            public float HorizontalWidth;
            public int Order;
        }

        private List<PropertyEntry> CollectPropertyEntries()
        {
            var entries = new List<PropertyEntry>();
            Type targetType = target.GetType();

            SerializedProperty prop = serializedObject.GetIterator();
            if (!prop.NextVisible(true)) return entries;

            do
            {
                if (prop.name == "m_Script") continue;

                FieldInfo field = FindField(targetType, prop.name);
                var entry = new PropertyEntry
                {
                    Property = prop.Copy(),
                    Order = 0
                };

                if (field != null)
                {
                    var boxGroup = field.GetCustomAttribute<BoxGroupAttribute>();
                    if (boxGroup != null) entry.BoxGroup = boxGroup.GroupName;

                    var foldoutGroup = field.GetCustomAttribute<FoldoutGroupAttribute>();
                    if (foldoutGroup != null) entry.FoldoutGroup = foldoutGroup.GroupName;

                    var tabGroup = field.GetCustomAttribute<TabGroupAttribute>();
                    if (tabGroup != null) entry.TabGroup = tabGroup.TabName;

                    var horizGroup = field.GetCustomAttribute<HorizontalGroupAttribute>();
                    if (horizGroup != null)
                    {
                        entry.HorizontalGroup = horizGroup.GroupName;
                        entry.HorizontalWidth = horizGroup.Width;
                    }

                    var orderAttr = field.GetCustomAttribute<PropertyOrderAttribute>();
                    if (orderAttr != null) entry.Order = orderAttr.Order;
                }

                entries.Add(entry);
            }
            while (prop.NextVisible(false));

            entries.Sort((a, b) => a.Order.CompareTo(b.Order));
            return entries;
        }

        private static FieldInfo FindField(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName, MemberFlags);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

        #endregion

        #region Property Drawing

        private void DrawPropertyEntries(List<PropertyEntry> entries)
        {
            // Group tracking
            string currentBox = null;
            string currentFoldout = null;
            string currentTab = null;
            string currentHoriz = null;
            var tabGroups = entries.Where(e => e.TabGroup != null)
                                  .Select(e => e.TabGroup).Distinct().ToList();

            // Draw tab bar if there are tabs
            string activeTab = null;
            if (tabGroups.Count > 0)
            {
                string key = target.GetType().FullName + "_tabs";
                if (!TabStates.ContainsKey(key)) TabStates[key] = 0;

                string[] tabNames = tabGroups.ToArray();
                TabStates[key] = GUILayout.Toolbar(TabStates[key], tabNames);
                if (TabStates[key] < tabNames.Length)
                    activeTab = tabNames[TabStates[key]];

                EditorGUILayout.Space(4);
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                // Skip tab-grouped properties not in active tab
                if (entry.TabGroup != null && entry.TabGroup != activeTab)
                    continue;

                // Box Group transitions
                if (entry.BoxGroup != currentBox)
                {
                    if (currentBox != null) EndBox();
                    if (entry.BoxGroup != null) BeginBox(entry.BoxGroup);
                    currentBox = entry.BoxGroup;
                }

                // Foldout Group transitions
                if (entry.FoldoutGroup != currentFoldout)
                {
                    if (currentFoldout != null) EndFoldout();
                    if (entry.FoldoutGroup != null)
                    {
                        if (!BeginFoldout(entry.FoldoutGroup))
                        {
                            currentFoldout = entry.FoldoutGroup;
                            // Skip all entries in this foldout
                            continue;
                        }
                    }
                    currentFoldout = entry.FoldoutGroup;
                }
                else if (currentFoldout != null)
                {
                    string key = target.GetType().FullName + "_foldout_" + currentFoldout;
                    if (FoldoutStates.ContainsKey(key) && !FoldoutStates[key])
                        continue;
                }

                // Horizontal Group
                if (entry.HorizontalGroup != currentHoriz)
                {
                    if (currentHoriz != null) EndHorizontal();
                    if (entry.HorizontalGroup != null) BeginHorizontal();
                    currentHoriz = entry.HorizontalGroup;
                }

                // Draw the property
                if (entry.HorizontalGroup != null && entry.HorizontalWidth > 0)
                    GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * entry.HorizontalWidth));
                else if (entry.HorizontalGroup != null)
                    GUILayout.BeginVertical();

                EditorGUILayout.PropertyField(entry.Property, true);

                if (entry.HorizontalGroup != null)
                    GUILayout.EndVertical();
            }

            // Close any open groups
            if (currentHoriz != null) EndHorizontal();
            if (currentFoldout != null) EndFoldout();
            if (currentBox != null) EndBox();
        }

        #endregion

        #region Group Helpers

        private void BeginBox(string title)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!string.IsNullOrEmpty(title))
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                DrawSeparator();
            }
        }

        private void EndBox()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private bool BeginFoldout(string title)
        {
            string key = target.GetType().FullName + "_foldout_" + title;
            if (!FoldoutStates.ContainsKey(key)) FoldoutStates[key] = true;

            FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], title, true, EditorStyles.foldoutHeader);

            if (FoldoutStates[key])
            {
                EditorGUI.indentLevel++;
            }

            return FoldoutStates[key];
        }

        private void EndFoldout()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void BeginHorizontal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        private void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin
                ? new Color(0.3f, 0.3f, 0.3f)
                : new Color(0.7f, 0.7f, 0.7f));
        }

        #endregion

        #region ShowInInspector

        private struct ShowInInspectorEntry
        {
            public string Name;
            public Func<object> Getter;
            public Type ValueType;
        }

        private static List<ShowInInspectorEntry> CollectShowInInspector(UnityEngine.Object target)
        {
            var entries = new List<ShowInInspectorEntry>();
            if (target == null) return entries;

            Type type = target.GetType();

            // Fields
            foreach (var field in type.GetFields(MemberFlags))
            {
                if (field.GetCustomAttribute<ShowInInspectorAttribute>() != null)
                {
                    var f = field;
                    entries.Add(new ShowInInspectorEntry
                    {
                        Name = ObjectNames.NicifyVariableName(f.Name),
                        Getter = () => f.GetValue(target),
                        ValueType = f.FieldType
                    });
                }
            }

            // Properties
            foreach (var prop in type.GetProperties(MemberFlags))
            {
                if (prop.GetCustomAttribute<ShowInInspectorAttribute>() != null && prop.CanRead)
                {
                    var p = prop;
                    entries.Add(new ShowInInspectorEntry
                    {
                        Name = ObjectNames.NicifyVariableName(p.Name),
                        Getter = () => p.GetValue(target),
                        ValueType = p.PropertyType
                    });
                }
            }

            return entries;
        }

        private void DrawShowInInspector()
        {
            if (_showInInspectorEntries == null || _showInInspectorEntries.Count == 0) return;

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Runtime Properties", EditorStyles.boldLabel);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;

            foreach (var entry in _showInInspectorEntries)
            {
                try
                {
                    object value = entry.Getter();
                    string display = value != null ? value.ToString() : "(null)";
                    EditorGUILayout.TextField(entry.Name, display);
                }
                catch
                {
                    EditorGUILayout.TextField(entry.Name, "(error)");
                }
            }

            GUI.enabled = wasEnabled;
        }

        #endregion
    }

    #region Button Utility (preserved)

    public struct ButtonMethodInfo
    {
        public MethodInfo Method;
        public ButtonAttribute Attribute;
        public string DisplayName;
    }

    public static class ButtonDrawerUtility
    {
        public static List<ButtonMethodInfo> CollectButtonMethods(UnityEngine.Object target)
        {
            var list = new List<ButtonMethodInfo>();
            if (target == null) return list;

            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                var displayName = string.IsNullOrEmpty(attr.Name) ? FormatMethodName(method.Name) : attr.Name;
                list.Add(new ButtonMethodInfo { Method = method, Attribute = attr, DisplayName = displayName });
            }

            return list;
        }

        public static void DrawButtons(List<ButtonMethodInfo> buttons, UnityEngine.Object[] targets)
        {
            if (buttons == null || buttons.Count == 0) return;

            EditorGUILayout.Space(4);

            foreach (var button in buttons)
            {
                bool isEnabled = IsButtonEnabled(button.Attribute.Mode);
                EditorGUI.BeginDisabledGroup(!isEnabled);

                if (GUILayout.Button(button.DisplayName))
                {
                    foreach (var t in targets)
                        button.Method.Invoke(t, null);
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        private static bool IsButtonEnabled(ButtonMode mode)
        {
            switch (mode)
            {
                case ButtonMode.EnabledInPlayMode: return Application.isPlaying;
                case ButtonMode.DisabledInPlayMode: return !Application.isPlaying;
                default: return true;
            }
        }

        /// <summary>Converts "MyMethodName" to "My Method Name".</summary>
        public static string FormatMethodName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            var chars = new List<char> { name[0] };
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                    chars.Add(' ');
                chars.Add(name[i]);
            }
            return new string(chars.ToArray());
        }
    }

    #endregion
}
#endif
