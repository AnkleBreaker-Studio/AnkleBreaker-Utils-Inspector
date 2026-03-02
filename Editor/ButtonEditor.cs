#if !ODIN_INSPECTOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Custom editor that draws [Button] attributed methods as clickable buttons
    /// below the default inspector. Only active when Odin Inspector is not present.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class ButtonMonoBehaviourEditor : UnityEditor.Editor
    {
        private List<ButtonMethodInfo> _buttonMethods;

        private void OnEnable() => _buttonMethods = ButtonDrawerUtility.CollectButtonMethods(target);

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ButtonDrawerUtility.DrawButtons(_buttonMethods, targets);
        }
    }

    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ButtonScriptableObjectEditor : UnityEditor.Editor
    {
        private List<ButtonMethodInfo> _buttonMethods;

        private void OnEnable() => _buttonMethods = ButtonDrawerUtility.CollectButtonMethods(target);

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ButtonDrawerUtility.DrawButtons(_buttonMethods, targets);
        }
    }

    public struct ButtonMethodInfo
    {
        public MethodInfo Method;
        public ButtonAttribute Attribute;
        public string DisplayName;
    }

    public static class ButtonDrawerUtility
    {
        public static List<ButtonMethodInfo> CollectButtonMethods(Object target)
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

        public static void DrawButtons(List<ButtonMethodInfo> buttons, Object[] targets)
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
        private static string FormatMethodName(string name)
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
}
#endif