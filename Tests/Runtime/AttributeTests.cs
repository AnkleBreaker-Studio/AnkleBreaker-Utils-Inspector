using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Tests
{
    public class AttributeTests
    {
        #region Test Helpers

        private class ShowIfTestComponent : MonoBehaviour
        {
            public bool isVisible = true;

            [ShowIf("isVisible")]
            public string conditionalField;
        }

        private class HideIfTestComponent : MonoBehaviour
        {
            public bool isHidden = false;

            [HideIf("isHidden")]
            public string conditionalField;
        }

        private class ButtonTestComponent : MonoBehaviour
        {
            public bool wasCalled;

            [Button]
            public void DefaultButton() => wasCalled = true;

            [Button("Custom Label")]
            public void NamedButton() { }

            [Button(Mode = ButtonMode.EnabledInPlayMode)]
            public void PlayModeOnly() { }
        }

        private class MultiAttributeTestComponent : MonoBehaviour
        {
            [HelpBox("This is info", HelpBoxAttribute.MessageType.Info)]
            public string infoField;

            [LabelText("Custom Name")]
            public string labeledField;

            [HideInNormalInspector]
            public string hiddenField;

            [HideVariableName]
            public string noNameField;

            [ReadOnly]
            public string readonlyField;

            [FolderPath("Assets/Default")]
            public string folderField;

            [ABToolTip("tipMethod")]
            public string tooltipField;

            [AutoCompleteText(new[] { "key1", "key2" }, new[] { "tip1", "tip2" })]
            public string autoCompleteField;
        }

        #endregion

        #region ButtonAttribute Tests

        [Test]
        public void ButtonAttribute_DefaultConstructor_NameIsNull()
        {
            var attr = new ButtonAttribute();
            Assert.IsNull(attr.Name);
            Assert.AreEqual(ButtonMode.AlwaysEnabled, attr.Mode);
        }

        [Test]
        public void ButtonAttribute_WithName_StoresName()
        {
            var attr = new ButtonAttribute("My Button");
            Assert.AreEqual("My Button", attr.Name);
        }

        [Test]
        public void ButtonAttribute_Mode_CanBeSet()
        {
            var attr = new ButtonAttribute { Mode = ButtonMode.EnabledInPlayMode };
            Assert.AreEqual(ButtonMode.EnabledInPlayMode, attr.Mode);
        }

        [Test]
        public void ButtonAttribute_TargetsMethodsOnly()
        {
            var usage = typeof(ButtonAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsNotNull(usage);
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
            Assert.IsFalse(usage.AllowMultiple);
        }

        [Test]
        public void ButtonAttribute_DetectedOnMethods()
        {
            var methods = typeof(ButtonTestComponent)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null)
                .ToList();

            Assert.AreEqual(3, methods.Count);
        }

        [Test]
        public void ButtonAttribute_NamedButton_HasCustomLabel()
        {
            var method = typeof(ButtonTestComponent).GetMethod("NamedButton");
            var attr = method.GetCustomAttribute<ButtonAttribute>();
            Assert.AreEqual("Custom Label", attr.Name);
        }

        #endregion

        #region ShowIf / HideIf Tests

        [Test]
        public void ShowIfAttribute_StoresConditionName()
        {
            var attr = new ShowIfAttribute("myBool");
            Assert.AreEqual("myBool", attr.ConditionName);
        }

        [Test]
        public void HideIfAttribute_StoresConditionName()
        {
            var attr = new HideIfAttribute("myBool");
            Assert.AreEqual("myBool", attr.ConditionName);
        }

        [Test]
        public void ShowIfAttribute_DetectedOnField()
        {
            var field = typeof(ShowIfTestComponent).GetField("conditionalField");
            var attr = field.GetCustomAttribute<ShowIfAttribute>();
            Assert.IsNotNull(attr);
            Assert.AreEqual("isVisible", attr.ConditionName);
        }

        [Test]
        public void HideIfAttribute_DetectedOnField()
        {
            var field = typeof(HideIfTestComponent).GetField("conditionalField");
            var attr = field.GetCustomAttribute<HideIfAttribute>();
            Assert.IsNotNull(attr);
            Assert.AreEqual("isHidden", attr.ConditionName);
        }

        #endregion

        #region HelpBox Tests

        [Test]
        public void HelpBoxAttribute_StoresMessageAndType()
        {
            var attr = new HelpBoxAttribute("Test message", HelpBoxAttribute.MessageType.Warning);
            Assert.AreEqual("Test message", attr.Message);
            Assert.AreEqual(HelpBoxAttribute.MessageType.Warning, attr.Type);
        }

        [Test]
        public void HelpBoxAttribute_DefaultType_IsInfo()
        {
            var attr = new HelpBoxAttribute("Test");
            Assert.AreEqual(HelpBoxAttribute.MessageType.Info, attr.Type);
        }

        #endregion

        #region LabelText Tests

        [Test]
        public void LabelTextAttribute_StoresDisplayName()
        {
            var attr = new LabelTextAttribute("My Label");
            Assert.AreEqual("My Label", attr.DisplayName);
        }

        [Test]
        public void LabelTextAttribute_AllowsMultiple()
        {
            var usage = typeof(LabelTextAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsNotNull(usage);
            Assert.IsTrue(usage.AllowMultiple);
        }

        #endregion

        #region FolderPath Tests

        [Test]
        public void FolderPathAttribute_StoresDefaultPath()
        {
            var attr = new FolderPathAttribute("Assets/MyFolder");
            Assert.AreEqual("Assets/MyFolder", attr.DefaultPath);
        }

        [Test]
        public void FolderPathAttribute_DefaultPath_IsEmpty()
        {
            var attr = new FolderPathAttribute();
            Assert.AreEqual("", attr.DefaultPath);
        }

        #endregion

        #region Other Attributes Tests

        [Test]
        public void HideInNormalInspectorAttribute_IsPropertyAttribute()
        {
            var attr = new HideInNormalInspectorAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void HideVariableNameAttribute_IsPropertyAttribute()
        {
            var attr = new HideVariableNameAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void ReadOnlyAttribute_IsPropertyAttribute()
        {
            var attr = new ReadOnlyAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void ABToolTipAttribute_StoresTooltipMemberName()
        {
            var attr = new ABToolTipAttribute("myTooltipMethod");
            Assert.AreEqual("myTooltipMethod", attr.TooltipMemberName);
        }

        [Test]
        public void EnumDescriptionAttribute_StoresDescription()
        {
            var attr = new EnumDescriptionAttribute("A description");
            Assert.AreEqual("A description", attr.Description);
        }

        #endregion

        #region AutoCompleteText Tests

        [Test]
        public void AutoCompleteTextAttribute_StoresKeysAndTooltips()
        {
            var keys = new[] { "key1", "key2", "key3" };
            var tips = new[] { "tip1", "tip2", "tip3" };
            var attr = new AutoCompleteTextAttribute(keys, tips);

            Assert.AreEqual(keys, attr.Keys);
            Assert.AreEqual(tips, attr.ToolTips);
        }

        [Test]
        public void AutoCompleteTextAttribute_NullTooltips_DefaultsToKeysLength()
        {
            var attr = new AutoCompleteTextAttribute(new[] { "a" });
            Assert.IsNotNull(attr.ToolTips);
            Assert.AreEqual(1, attr.ToolTips.Length);
        }

        [Test]
        public void AutoCompleteTextAttribute_MinMaxLines()
        {
            var attr = new AutoCompleteTextAttribute(new[] { "a" }, minLines: 3, maxLines: 10);
            Assert.AreEqual(3, attr.MinLines);
            Assert.AreEqual(10, attr.MaxLines);
        }

        #endregion

        #region FreeRange Tests

        [Test]
        public void FreeRangeAttribute_StoresMinMax()
        {
            var attr = new FreeRangeAttribute(0f, 100f);
            Assert.AreEqual(0f, attr.Min);
            Assert.AreEqual(100f, attr.Max);
        }

        [Test]
        public void FreeRangeAttribute_NegativeRange()
        {
            var attr = new FreeRangeAttribute(-50f, 50f);
            Assert.AreEqual(-50f, attr.Min);
            Assert.AreEqual(50f, attr.Max);
        }

        [Test]
        public void FreeRangeAttribute_IsPropertyAttribute()
        {
            var attr = new FreeRangeAttribute(0f, 1f);
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region Required Tests

        [Test]
        public void RequiredAttribute_DefaultMessage_IsNull()
        {
            var attr = new RequiredAttribute();
            Assert.IsNull(attr.Message);
        }

        [Test]
        public void RequiredAttribute_CustomMessage_Stored()
        {
            var attr = new RequiredAttribute("This field is mandatory!");
            Assert.AreEqual("This field is mandatory!", attr.Message);
        }

        [Test]
        public void RequiredAttribute_IsPropertyAttribute()
        {
            var attr = new RequiredAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region ToggleButton Tests

        [Test]
        public void ToggleButtonAttribute_DefaultLabels_AreNull()
        {
            var attr = new ToggleButtonAttribute();
            Assert.IsNull(attr.TrueLabel);
            Assert.IsNull(attr.FalseLabel);
        }

        [Test]
        public void ToggleButtonAttribute_SingleLabel()
        {
            var attr = new ToggleButtonAttribute("Active");
            Assert.AreEqual("Active", attr.TrueLabel);
            Assert.IsNull(attr.FalseLabel);
        }

        [Test]
        public void ToggleButtonAttribute_DualLabels()
        {
            var attr = new ToggleButtonAttribute("ON", "OFF");
            Assert.AreEqual("ON", attr.TrueLabel);
            Assert.AreEqual("OFF", attr.FalseLabel);
        }

        [Test]
        public void ToggleButtonAttribute_IsPropertyAttribute()
        {
            var attr = new ToggleButtonAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region SerializedDictionary Tests

        [Test]
        public void SerializedDictionary_CanAddAndRetrieve()
        {
            var dict = new AB_SerializedDictionary<string, int>();
            dict.Add("health", 100);
            dict.Add("mana", 50);

            Assert.AreEqual(100, dict["health"]);
            Assert.AreEqual(50, dict["mana"]);
            Assert.AreEqual(2, dict.Count);
        }

        [Test]
        public void SerializedDictionary_ContainsKey()
        {
            var dict = new AB_SerializedDictionary<string, int>();
            dict.Add("key", 42);

            Assert.IsTrue(dict.ContainsKey("key"));
            Assert.IsFalse(dict.ContainsKey("missing"));
        }

        #endregion
    }
}