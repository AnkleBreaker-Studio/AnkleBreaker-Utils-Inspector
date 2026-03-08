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
            InvokeLifecycleMethods<OnInspectorInitAttribute>(target);
        }

        private void OnDisable()
        {
            InvokeLifecycleMethods<OnInspectorDisposeAttribute>(target);
        }

        private static void InvokeLifecycleMethods<T>(UnityEngine.Object target) where T : Attribute
        {
            if (target == null) return;
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<T>() != null && m.GetParameters().Length == 0);
            foreach (var method in methods)
            {
                try { method.Invoke(target, null); }
                catch (System.Exception e) { Debug.LogException(e); }
            }
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
            public bool BoxFoldable;
            public string BoxShowIf;
            public string FoldoutGroup;
            public FoldoutGroupStyle FoldoutStyle;
            public Color? FoldoutColor;
            public string TabGroup;
            public string HorizontalGroup;
            public float HorizontalWidth;
            public int Order;
            public int OriginalIndex;

            // Conditional visibility / enable (handled at editor level, not drawer)
            public ShowIfAttribute ShowIf;
            public HideIfAttribute HideIf;
            public EnableIfAttribute EnableIf;

            // SectionHeader (handled at editor level to avoid conflict with HorizontalGroup)
            public string SectionHeaderTitle;
            public SectionHeaderStyle SectionHeaderStyle;
            public Color? SectionHeaderColor;
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
                    if (boxGroup != null)
                    {
                        entry.BoxGroup = boxGroup.GroupName;
                        entry.BoxFoldable = boxGroup.Foldable;
                        entry.BoxShowIf = boxGroup.ShowIf;
                    }

                    var foldoutGroup = field.GetCustomAttribute<FoldoutGroupAttribute>();
                    if (foldoutGroup != null)
                    {
                        entry.FoldoutGroup = foldoutGroup.GroupName;
                        entry.FoldoutStyle = foldoutGroup.Style;
                        if (foldoutGroup.HasCustomColor)
                            entry.FoldoutColor = new Color(foldoutGroup.R, foldoutGroup.G, foldoutGroup.B);
                    }

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

                    // Conditional attributes (evaluated at editor level to free up PropertyDrawer slot)
                    entry.ShowIf = field.GetCustomAttribute<ShowIfAttribute>();
                    entry.HideIf = field.GetCustomAttribute<HideIfAttribute>();
                    entry.EnableIf = field.GetCustomAttribute<EnableIfAttribute>();

                    // SectionHeader (drawn at editor level to avoid conflict with HorizontalGroup)
                    var sectionHeader = field.GetCustomAttribute<SectionHeaderAttribute>();
                    if (sectionHeader != null)
                    {
                        entry.SectionHeaderTitle = sectionHeader.Title;
                        entry.SectionHeaderStyle = sectionHeader.Style;
                        if (sectionHeader.HasCustomColor)
                            entry.SectionHeaderColor = new Color(sectionHeader.R, sectionHeader.G, sectionHeader.B);
                    }
                }

                entry.OriginalIndex = entries.Count;
                entries.Add(entry);
            }
            while (prop.NextVisible(false));

            entries.Sort((a, b) =>
            {
                int c = a.Order.CompareTo(b.Order);
                return c != 0 ? c : a.OriginalIndex.CompareTo(b.OriginalIndex);
            });
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
            bool boxHidden = false;
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
                    if (currentBox != null && !boxHidden) EndBox();
                    boxHidden = false;
                    if (entry.BoxGroup != null)
                    {
                        // Conditional BoxGroup: evaluate ShowIf before drawing
                        if (!string.IsNullOrEmpty(entry.BoxShowIf))
                        {
                            bool boxVisible = ConditionResolver.Evaluate(entry.Property, entry.BoxShowIf, false);
                            if (!boxVisible)
                            {
                                boxHidden = true;
                                currentBox = entry.BoxGroup;
                                continue;
                            }
                        }
                        BeginBox(entry.BoxGroup, entry.BoxFoldable);
                    }
                    currentBox = entry.BoxGroup;
                }

                // Skip all entries in a hidden conditional BoxGroup
                if (boxHidden) continue;

                // Skip box content if foldable box is collapsed
                if (currentBox != null)
                {
                    string boxKey = target.GetType().FullName + "_box_" + currentBox;
                    if (FoldoutStates.ContainsKey(boxKey) && !FoldoutStates[boxKey])
                        continue;
                }

                // Foldout Group transitions
                if (entry.FoldoutGroup != currentFoldout)
                {
                    if (currentFoldout != null) EndFoldout();
                    if (entry.FoldoutGroup != null)
                    {
                        if (!BeginFoldout(entry.FoldoutGroup, entry.FoldoutStyle, entry.FoldoutColor))
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

                // --- SectionHeader: draw before horizontal group to avoid layout conflict ---
                if (entry.SectionHeaderTitle != null && entry.HorizontalGroup != null)
                {
                    // Close existing horizontal if any, draw header outside, then let transition reopen
                    if (currentHoriz != null) { EndHorizontal(); currentHoriz = null; }
                    SectionHeaderDrawer.DrawManualSectionHeader(entry.SectionHeaderTitle, entry.SectionHeaderStyle, entry.SectionHeaderColor);
                    SectionHeaderDrawer.SuppressNextDraw = true;
                }

                // --- Conditional visibility (ShowIf / HideIf) ---
                bool visible = true;
                if (entry.ShowIf != null)
                {
                    visible = entry.ShowIf.HasCompareValue
                        ? ConditionResolver.EvaluateComparison(entry.Property, entry.ShowIf.ConditionName, entry.ShowIf.CompareValue, false)
                        : ConditionResolver.Evaluate(entry.Property, entry.ShowIf.ConditionName, false);
                }
                if (entry.HideIf != null)
                {
                    bool hideResult = entry.HideIf.HasCompareValue
                        ? ConditionResolver.EvaluateComparison(entry.Property, entry.HideIf.ConditionName, entry.HideIf.CompareValue, false)
                        : ConditionResolver.Evaluate(entry.Property, entry.HideIf.ConditionName, false);
                    if (hideResult) visible = false;
                }

                if (!visible)
                {
                    // If inside a horizontal group, still need to account for layout
                    if (entry.HorizontalGroup != null)
                    {
                        // Draw an invisible placeholder to keep horizontal layout stable
                        // (skip entirely — the group will still close properly)
                    }
                    continue;
                }

                // --- Conditional enable (EnableIf) ---
                bool wasEnabled = GUI.enabled;
                if (entry.EnableIf != null)
                {
                    bool enableResult = entry.EnableIf.HasCompareValue
                        ? ConditionResolver.EvaluateComparison(entry.Property, entry.EnableIf.ConditionName, entry.EnableIf.CompareValue, true)
                        : ConditionResolver.Evaluate(entry.Property, entry.EnableIf.ConditionName, true);
                    GUI.enabled = enableResult;
                }

                // Draw the property
                if (entry.HorizontalGroup != null && entry.HorizontalWidth > 0)
                    GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * entry.HorizontalWidth));
                else if (entry.HorizontalGroup != null)
                    GUILayout.BeginVertical();

                EditorGUILayout.PropertyField(entry.Property, true);

                if (entry.HorizontalGroup != null)
                    GUILayout.EndVertical();

                // Restore enable state
                if (entry.EnableIf != null)
                    GUI.enabled = wasEnabled;
            }

            // Close any open groups
            if (currentHoriz != null) EndHorizontal();
            if (currentFoldout != null) EndFoldout();
            if (currentBox != null && !boxHidden) EndBox();
        }

        #endregion

        #region Group Helpers

        private static GUIStyle _boxTitleStyle;
        private static GUIStyle BoxTitleStyle
        {
            get
            {
                if (_boxTitleStyle == null)
                {
                    _boxTitleStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize = 13
                    };
                }
                return _boxTitleStyle;
            }
        }

        private static GUIStyle _boxFoldoutStyle;
        private static GUIStyle BoxFoldoutStyle
        {
            get
            {
                if (_boxFoldoutStyle == null)
                {
                    _boxFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                    {
                        fontStyle = FontStyle.Bold,
                        fontSize = 13
                    };
                }
                return _boxFoldoutStyle;
            }
        }

        private void BeginBox(string title, bool foldable = false)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!string.IsNullOrEmpty(title))
            {
                if (foldable)
                {
                    string key = target.GetType().FullName + "_box_" + title;
                    if (!FoldoutStates.ContainsKey(key)) FoldoutStates[key] = true;
                    FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], title, true, BoxFoldoutStyle);
                }
                else
                {
                    EditorGUILayout.LabelField(title, BoxTitleStyle);
                    DrawSeparator();
                }
            }
        }

        private void EndBox()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private bool BeginFoldout(string title, FoldoutGroupStyle style = FoldoutGroupStyle.Default, Color? color = null)
        {
            string key = target.GetType().FullName + "_foldout_" + title;
            if (!FoldoutStates.ContainsKey(key)) FoldoutStates[key] = true;

            switch (style)
            {
                case FoldoutGroupStyle.Line:
                    FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], "", true, EditorStyles.foldout);
                    var lineRect = GUILayoutUtility.GetLastRect();
                    lineRect.xMin += 12f;
                    var prevColor = GUI.color;
                    GUI.color = color.HasValue ? color.Value : GUI.color;
                    EditorGUI.LabelField(lineRect, title, EditorStyles.boldLabel);
                    GUI.color = prevColor;
                    if (FoldoutStates[key])
                    {
                        Rect sep = EditorGUILayout.GetControlRect(false, 1f);
                        EditorGUI.DrawRect(sep, EditorGUIUtility.isProSkin ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.7f, 0.7f, 0.7f));
                    }
                    break;

                case FoldoutGroupStyle.CenterLine:
                    FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], "", true, EditorStyles.foldout);
                    var clRect = GUILayoutUtility.GetLastRect();
                    float textWidth = EditorStyles.boldLabel.CalcSize(new GUIContent(title)).x;
                    float centerX = clRect.x + clRect.width * 0.5f;
                    float lineY = clRect.y + clRect.height * 0.5f;
                    Color lineCol = color ?? (EditorGUIUtility.isProSkin ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f));
                    EditorGUI.DrawRect(new Rect(clRect.x + 12f, lineY, centerX - textWidth * 0.5f - clRect.x - 16f, 1f), lineCol);
                    EditorGUI.DrawRect(new Rect(centerX + textWidth * 0.5f + 4f, lineY, clRect.xMax - centerX - textWidth * 0.5f - 4f, 1f), lineCol);
                    var titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
                    if (color.HasValue) titleStyle.normal.textColor = color.Value;
                    EditorGUI.LabelField(new Rect(clRect.x + 12f, clRect.y, clRect.width - 12f, clRect.height), title, titleStyle);
                    break;

                case FoldoutGroupStyle.Clean:
                    FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], "", true, EditorStyles.foldout);
                    var cleanRect = GUILayoutUtility.GetLastRect();
                    cleanRect.xMin += 12f;
                    var cleanStyle = new GUIStyle(EditorStyles.boldLabel);
                    if (color.HasValue) cleanStyle.normal.textColor = color.Value;
                    EditorGUI.LabelField(cleanRect, title, cleanStyle);
                    break;

                default:
                    FoldoutStates[key] = EditorGUILayout.Foldout(FoldoutStates[key], title, true, EditorStyles.foldoutHeader);
                    break;
            }

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
            public bool RuntimeOnly;
        }

        private static List<ShowInInspectorEntry> CollectShowInInspector(UnityEngine.Object target)
        {
            var entries = new List<ShowInInspectorEntry>();
            if (target == null) return entries;

            Type type = target.GetType();

            // Fields
            foreach (var field in type.GetFields(MemberFlags))
            {
                var attr = field.GetCustomAttribute<ShowInInspectorAttribute>();
                if (attr != null)
                {
                    var f = field;
                    entries.Add(new ShowInInspectorEntry
                    {
                        Name = ObjectNames.NicifyVariableName(f.Name),
                        Getter = () => f.GetValue(target),
                        ValueType = f.FieldType,
                        RuntimeOnly = attr.RuntimeOnly
                    });
                }
            }

            // Properties
            foreach (var prop in type.GetProperties(MemberFlags))
            {
                var attr = prop.GetCustomAttribute<ShowInInspectorAttribute>();
                if (attr != null && prop.CanRead)
                {
                    var p = prop;
                    entries.Add(new ShowInInspectorEntry
                    {
                        Name = ObjectNames.NicifyVariableName(p.Name),
                        Getter = () => p.GetValue(target),
                        ValueType = p.PropertyType,
                        RuntimeOnly = attr.RuntimeOnly
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
                if (entry.RuntimeOnly && !Application.isPlaying)
                    continue;

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
        public ParameterInfo[] Parameters;
        public object[] ParameterValues;
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
                var parameters = method.GetParameters();
                object[] paramValues = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    paramValues[i] = parameters[i].HasDefaultValue ? parameters[i].DefaultValue : GetDefaultValue(parameters[i].ParameterType);
                }
                list.Add(new ButtonMethodInfo { Method = method, Attribute = attr, DisplayName = displayName, Parameters = parameters, ParameterValues = paramValues });
            }

            return list;
        }

        public static void DrawButtons(List<ButtonMethodInfo> buttons, UnityEngine.Object[] targets)
        {
            if (buttons == null || buttons.Count == 0) return;

            EditorGUILayout.Space(4);

            string currentGroup = null;
            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                string group = button.Attribute.HorizontalGroup;

                // Begin horizontal group
                if (group != null && group != currentGroup)
                {
                    EditorGUILayout.BeginHorizontal();
                    currentGroup = group;
                }

                bool isEnabled = IsButtonEnabled(button.Attribute.Mode);
                EditorGUI.BeginDisabledGroup(!isEnabled);

                // Draw parameter fields if method has parameters
                if (button.Parameters != null && button.Parameters.Length > 0)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    for (int p = 0; p < button.Parameters.Length; p++)
                    {
                        button.ParameterValues[p] = DrawParameterField(button.Parameters[p], button.ParameterValues[p]);
                    }
                }

                if (GUILayout.Button(button.DisplayName))
                {
                    foreach (var t in targets)
                        button.Method.Invoke(t, button.Parameters.Length > 0 ? button.ParameterValues : null);
                }

                if (button.Parameters != null && button.Parameters.Length > 0)
                    EditorGUILayout.EndVertical();

                EditorGUI.EndDisabledGroup();

                // End horizontal group if next button is different group or last button
                bool endGroup = currentGroup != null;
                if (endGroup)
                {
                    string nextGroup = (i + 1 < buttons.Count) ? buttons[i + 1].Attribute.HorizontalGroup : null;
                    if (nextGroup != currentGroup)
                    {
                        EditorGUILayout.EndHorizontal();
                        currentGroup = null;
                    }
                }
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

        internal static object GetDefaultValue(System.Type type)
        {
            if (type == typeof(string)) return "";
            if (type == typeof(int)) return 0;
            if (type == typeof(float)) return 0f;
            if (type == typeof(bool)) return false;
            if (type == typeof(Vector2)) return Vector2.zero;
            if (type == typeof(Vector3)) return Vector3.zero;
            if (type == typeof(Color)) return Color.white;
            if (type.IsEnum) return System.Activator.CreateInstance(type);
            if (type.IsValueType) return System.Activator.CreateInstance(type);
            return null;
        }

        internal static object DrawParameterField(ParameterInfo param, object currentValue)
        {
            string label = ObjectNames.NicifyVariableName(param.Name);
            System.Type t = param.ParameterType;

            if (t == typeof(int))
                return EditorGUILayout.IntField(label, (int)(currentValue ?? 0));
            if (t == typeof(float))
                return EditorGUILayout.FloatField(label, (float)(currentValue ?? 0f));
            if (t == typeof(string))
                return EditorGUILayout.TextField(label, (string)(currentValue ?? ""));
            if (t == typeof(bool))
                return EditorGUILayout.Toggle(label, (bool)(currentValue ?? false));
            if (t == typeof(Vector2))
                return EditorGUILayout.Vector2Field(label, (Vector2)(currentValue ?? Vector2.zero));
            if (t == typeof(Vector3))
                return EditorGUILayout.Vector3Field(label, (Vector3)(currentValue ?? Vector3.zero));
            if (t == typeof(Color))
                return EditorGUILayout.ColorField(label, (Color)(currentValue ?? Color.white));
            if (t.IsEnum)
                return EditorGUILayout.EnumPopup(label, (System.Enum)(currentValue ?? System.Activator.CreateInstance(t)));
            if (typeof(UnityEngine.Object).IsAssignableFrom(t))
                return EditorGUILayout.ObjectField(label, (UnityEngine.Object)currentValue, t, true);

            EditorGUILayout.LabelField(label, $"[Unsupported type: {t.Name}]");
            return currentValue;
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
