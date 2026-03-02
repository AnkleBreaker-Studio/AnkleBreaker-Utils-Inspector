#if !ODIN_INSPECTOR
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using AnkleBreaker.Utils.Inspector.Editor;

namespace AnkleBreaker.Utils.Inspector.Editor.Tests
{
    public class ButtonDrawerTests
    {
        private class TestMono : MonoBehaviour
        {
            public bool called;

            [Button]
            public void NoLabel() => called = true;

            [Button("Do Something")]
            public void WithLabel() { }

            [Button(Mode = ButtonMode.DisabledInPlayMode)]
            public void EditorOnly() { }

            public void NotAButton() { }
        }

        [Test]
        public void CollectButtonMethods_FindsAllButtonMethods()
        {
            var go = new GameObject("Test");
            var mono = go.AddComponent<TestMono>();

            var buttons = ButtonDrawerUtility.CollectButtonMethods(mono);

            Assert.AreEqual(3, buttons.Count);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CollectButtonMethods_IgnoresNonButtonMethods()
        {
            var go = new GameObject("Test");
            var mono = go.AddComponent<TestMono>();

            var buttons = ButtonDrawerUtility.CollectButtonMethods(mono);
            var names = buttons.Select(b => b.Method.Name).ToList();

            Assert.IsFalse(names.Contains("NotAButton"));
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CollectButtonMethods_UsesCustomName()
        {
            var go = new GameObject("Test");
            var mono = go.AddComponent<TestMono>();

            var buttons = ButtonDrawerUtility.CollectButtonMethods(mono);
            var labeled = buttons.First(b => b.Method.Name == "WithLabel");

            Assert.AreEqual("Do Something", labeled.DisplayName);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CollectButtonMethods_FormatsMethodName_WhenNoLabel()
        {
            var go = new GameObject("Test");
            var mono = go.AddComponent<TestMono>();

            var buttons = ButtonDrawerUtility.CollectButtonMethods(mono);
            var noLabel = buttons.First(b => b.Method.Name == "NoLabel");

            Assert.AreEqual("No Label", noLabel.DisplayName);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CollectButtonMethods_NullTarget_ReturnsEmpty()
        {
            var buttons = ButtonDrawerUtility.CollectButtonMethods(null);
            Assert.IsNotNull(buttons);
            Assert.AreEqual(0, buttons.Count);
        }

        [Test]
        public void CollectButtonMethods_PreservesButtonMode()
        {
            var go = new GameObject("Test");
            var mono = go.AddComponent<TestMono>();

            var buttons = ButtonDrawerUtility.CollectButtonMethods(mono);
            var editorOnly = buttons.First(b => b.Method.Name == "EditorOnly");

            Assert.AreEqual(ButtonMode.DisabledInPlayMode, editorOnly.Attribute.Mode);
            Object.DestroyImmediate(go);
        }
    }
}
#endif