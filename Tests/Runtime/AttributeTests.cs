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
        public void ButtonAttribute_HorizontalGroup_DefaultNull()
        {
            var attr = new ButtonAttribute();
            Assert.IsNull(attr.HorizontalGroup);
        }

        [Test]
        public void ButtonAttribute_HorizontalGroup_CanBeSet()
        {
            var attr = new ButtonAttribute { HorizontalGroup = "Actions" };
            Assert.AreEqual("Actions", attr.HorizontalGroup);
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
            Assert.IsFalse(attr.HasCompareValue);
        }

        [Test]
        public void ShowIfAttribute_WithIntComparison()
        {
            var attr = new ShowIfAttribute("myEnum", 2);
            Assert.AreEqual("myEnum", attr.ConditionName);
            Assert.IsTrue(attr.HasCompareValue);
            Assert.AreEqual(2, attr.CompareValue);
        }

        [Test]
        public void ShowIfAttribute_WithStringComparison()
        {
            var attr = new ShowIfAttribute("myField", "hello");
            Assert.IsTrue(attr.HasCompareValue);
            Assert.AreEqual("hello", attr.CompareValue);
        }

        [Test]
        public void HideIfAttribute_StoresConditionName()
        {
            var attr = new HideIfAttribute("myBool");
            Assert.AreEqual("myBool", attr.ConditionName);
            Assert.IsFalse(attr.HasCompareValue);
        }

        [Test]
        public void HideIfAttribute_WithIntComparison()
        {
            var attr = new HideIfAttribute("mode", 1);
            Assert.IsTrue(attr.HasCompareValue);
            Assert.AreEqual(1, attr.CompareValue);
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

        #region EnableIf Tests

        [Test]
        public void EnableIfAttribute_StoresBoolCondition()
        {
            var attr = new EnableIfAttribute("isReady");
            Assert.AreEqual("isReady", attr.ConditionName);
            Assert.IsFalse(attr.HasCompareValue);
        }

        [Test]
        public void EnableIfAttribute_WithIntComparison()
        {
            var attr = new EnableIfAttribute("mode", 3);
            Assert.IsTrue(attr.HasCompareValue);
            Assert.AreEqual(3, attr.CompareValue);
        }

        [Test]
        public void EnableIfAttribute_WithStringComparison()
        {
            var attr = new EnableIfAttribute("type", "advanced");
            Assert.IsTrue(attr.HasCompareValue);
            Assert.AreEqual("advanced", attr.CompareValue);
        }

        #endregion

        #region ProgressBar Tests

        [Test]
        public void ProgressBarAttribute_StoresMinMax()
        {
            var attr = new ProgressBarAttribute(0f, 100f);
            Assert.AreEqual(0f, attr.Min);
            Assert.AreEqual(100f, attr.Max);
        }

        [Test]
        public void ProgressBarAttribute_DefaultColor()
        {
            var attr = new ProgressBarAttribute(0, 1);
            Assert.AreEqual(0.2f, attr.R, 0.01f);
            Assert.AreEqual(0.6f, attr.G, 0.01f);
            Assert.AreEqual(0.9f, attr.B, 0.01f);
        }

        [Test]
        public void ProgressBarAttribute_CustomColor()
        {
            var attr = new ProgressBarAttribute(0, 1, "HP", 1f, 0f, 0f);
            Assert.AreEqual("HP", attr.Label);
            Assert.AreEqual(1f, attr.R, 0.01f);
            Assert.AreEqual(0f, attr.G, 0.01f);
        }

        #endregion

        #region SectionHeader Tests

        [Test]
        public void SectionHeaderAttribute_StoresTitle()
        {
            var attr = new SectionHeaderAttribute("My Section");
            Assert.AreEqual("My Section", attr.Title);
            Assert.AreEqual(SectionHeaderStyle.CenterLine, attr.Style);
        }

        [Test]
        public void SectionHeaderAttribute_CustomStyle()
        {
            var attr = new SectionHeaderAttribute("Title", SectionHeaderStyle.Box);
            Assert.AreEqual(SectionHeaderStyle.Box, attr.Style);
        }

        [Test]
        public void SectionHeaderAttribute_AllStyles()
        {
            Assert.AreEqual(SectionHeaderStyle.CenterLine, new SectionHeaderAttribute("A").Style);
            Assert.AreEqual(SectionHeaderStyle.CenterLine, new SectionHeaderAttribute("B", SectionHeaderStyle.CenterLine).Style);
            Assert.AreEqual(SectionHeaderStyle.Box, new SectionHeaderAttribute("C", SectionHeaderStyle.Box).Style);
            Assert.AreEqual(SectionHeaderStyle.Clean, new SectionHeaderAttribute("D", SectionHeaderStyle.Clean).Style);
        }

        #endregion

        #region BoxGroup Tests

        [Test]
        public void BoxGroupAttribute_StoresGroupName()
        {
            var attr = new BoxGroupAttribute("Settings");
            Assert.AreEqual("Settings", attr.GroupName);
            Assert.IsFalse(attr.Foldable);
        }

        [Test]
        public void BoxGroupAttribute_Foldable()
        {
            var attr = new BoxGroupAttribute("Settings", foldable: true);
            Assert.IsTrue(attr.Foldable);
        }

        #endregion

        #region FoldoutGroup Tests

        [Test]
        public void FoldoutGroupAttribute_StoresGroupName()
        {
            var attr = new FoldoutGroupAttribute("Advanced");
            Assert.AreEqual("Advanced", attr.GroupName);
            Assert.AreEqual(FoldoutGroupStyle.Default, attr.Style);
            Assert.IsFalse(attr.HasCustomColor);
        }

        [Test]
        public void FoldoutGroupAttribute_StoresStyle()
        {
            var attr = new FoldoutGroupAttribute("Events", FoldoutGroupStyle.Box);
            Assert.AreEqual("Events", attr.GroupName);
            Assert.AreEqual(FoldoutGroupStyle.Box, attr.Style);
            Assert.IsFalse(attr.HasCustomColor);
        }

        [Test]
        public void FoldoutGroupAttribute_StoresStyleAndColor()
        {
            var attr = new FoldoutGroupAttribute("Net", FoldoutGroupStyle.CenterLine, 0.2f, 0.5f, 0.9f);
            Assert.AreEqual("Net", attr.GroupName);
            Assert.AreEqual(FoldoutGroupStyle.CenterLine, attr.Style);
            Assert.IsTrue(attr.HasCustomColor);
            Assert.AreEqual(0.2f, attr.R, 0.01f);
            Assert.AreEqual(0.5f, attr.G, 0.01f);
            Assert.AreEqual(0.9f, attr.B, 0.01f);
        }

        #endregion

        #region TabGroup Tests

        [Test]
        public void TabGroupAttribute_StoresTabName()
        {
            var attr = new TabGroupAttribute("General");
            Assert.AreEqual("General", attr.TabName);
        }

        #endregion

        #region HorizontalGroup Tests

        [Test]
        public void HorizontalGroupAttribute_StoresGroupName()
        {
            var attr = new HorizontalGroupAttribute("row1");
            Assert.AreEqual("row1", attr.GroupName);
            Assert.AreEqual(0f, attr.Width, 0.01f);
        }

        [Test]
        public void HorizontalGroupAttribute_CustomWidth()
        {
            var attr = new HorizontalGroupAttribute("row1", 0.5f);
            Assert.AreEqual(0.5f, attr.Width, 0.01f);
        }

        #endregion

        #region ValueDropdown Tests

        [Test]
        public void ValueDropdownAttribute_StoresMemberName()
        {
            var attr = new ValueDropdownAttribute("GetOptions");
            Assert.AreEqual("GetOptions", attr.MemberName);
        }

        #endregion

        #region OnValueChanged Tests

        [Test]
        public void OnValueChangedAttribute_StoresMethodName()
        {
            var attr = new OnValueChangedAttribute("OnChanged");
            Assert.AreEqual("OnChanged", attr.MethodName);
        }

        #endregion

        #region MinMaxSlider Tests

        [Test]
        public void MinMaxSliderAttribute_StoresMinMax()
        {
            var attr = new MinMaxSliderAttribute(0f, 10f);
            Assert.AreEqual(0f, attr.Min);
            Assert.AreEqual(10f, attr.Max);
        }

        #endregion

        #region InfoBox Tests

        [Test]
        public void InfoBoxAttribute_StoresMessage()
        {
            var attr = new InfoBoxAttribute("Warning!", InfoMessageType.Warning);
            Assert.AreEqual("Warning!", attr.Message);
            Assert.AreEqual(InfoMessageType.Warning, attr.Type);
        }

        [Test]
        public void InfoBoxAttribute_DefaultType()
        {
            var attr = new InfoBoxAttribute("Info");
            Assert.AreEqual(InfoMessageType.Info, attr.Type);
            Assert.IsNull(attr.VisibleIf);
        }

        [Test]
        public void InfoBoxAttribute_WithCondition()
        {
            var attr = new InfoBoxAttribute("Msg", InfoMessageType.Error, "showError");
            Assert.AreEqual("showError", attr.VisibleIf);
        }

        #endregion

        #region PropertyOrder Tests

        [Test]
        public void PropertyOrderAttribute_StoresOrder()
        {
            var attr = new PropertyOrderAttribute(5);
            Assert.AreEqual(5, attr.Order);
        }

        [Test]
        public void PropertyOrderAttribute_NegativeOrder()
        {
            var attr = new PropertyOrderAttribute(-10);
            Assert.AreEqual(-10, attr.Order);
        }

        #endregion

        #region PropertySpace Tests

        [Test]
        public void PropertySpaceAttribute_DefaultValues()
        {
            var attr = new PropertySpaceAttribute();
            Assert.AreEqual(8f, attr.SpaceBefore, 0.01f);
            Assert.AreEqual(0f, attr.SpaceAfter, 0.01f);
        }

        [Test]
        public void PropertySpaceAttribute_CustomValues()
        {
            var attr = new PropertySpaceAttribute(16f, 8f);
            Assert.AreEqual(16f, attr.SpaceBefore, 0.01f);
            Assert.AreEqual(8f, attr.SpaceAfter, 0.01f);
        }

        #endregion

        #region SuffixLabel Tests

        [Test]
        public void SuffixLabelAttribute_StoresSuffix()
        {
            var attr = new SuffixLabelAttribute("ms");
            Assert.AreEqual("ms", attr.Suffix);
        }

        #endregion

        #region EnumToggleButtons Tests

        [Test]
        public void EnumToggleButtonsAttribute_IsPropertyAttribute()
        {
            var attr = new EnumToggleButtonsAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region PreviewField Tests

        [Test]
        public void PreviewFieldAttribute_DefaultHeight()
        {
            var attr = new PreviewFieldAttribute();
            Assert.AreEqual(64f, attr.PreviewHeight, 0.01f);
        }

        [Test]
        public void PreviewFieldAttribute_CustomHeight()
        {
            var attr = new PreviewFieldAttribute(128f);
            Assert.AreEqual(128f, attr.PreviewHeight, 0.01f);
        }

        #endregion

        #region ValidateInput Tests

        [Test]
        public void ValidateInputAttribute_StoresValues()
        {
            var attr = new ValidateInputAttribute("IsValid", "Must be valid!", ValidateMessageType.Warning);
            Assert.AreEqual("IsValid", attr.ValidatorMethod);
            Assert.AreEqual("Must be valid!", attr.Message);
            Assert.AreEqual(ValidateMessageType.Warning, attr.MessageType);
        }

        [Test]
        public void ValidateInputAttribute_DefaultMessage()
        {
            var attr = new ValidateInputAttribute("Check");
            Assert.AreEqual("Validation failed", attr.Message);
            Assert.AreEqual(ValidateMessageType.Error, attr.MessageType);
        }

        #endregion

        #region ShowInInspector Tests

        [Test]
        public void ShowInInspectorAttribute_IsRegularAttribute()
        {
            var attr = new ShowInInspectorAttribute();
            Assert.IsInstanceOf<Attribute>(attr);
            Assert.IsNotInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void ShowInInspectorAttribute_TargetsFieldsAndProperties()
        {
            var usage = typeof(ShowInInspectorAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsNotNull(usage);
            Assert.IsTrue((usage.ValidOn & AttributeTargets.Field) != 0);
            Assert.IsTrue((usage.ValidOn & AttributeTargets.Property) != 0);
        }

        [Test]
        public void ShowInInspectorAttribute_RuntimeOnly_DefaultFalse()
        {
            var attr = new ShowInInspectorAttribute();
            Assert.IsFalse(attr.RuntimeOnly);
        }

        [Test]
        public void ShowInInspectorAttribute_RuntimeOnly_True()
        {
            var attr = new ShowInInspectorAttribute(runtimeOnly: true);
            Assert.IsTrue(attr.RuntimeOnly);
        }

        #endregion

        #region InlineEditor Tests

        [Test]
        public void InlineEditorAttribute_DefaultDrawHeader()
        {
            var attr = new InlineEditorAttribute();
            Assert.IsTrue(attr.DrawHeader);
        }

        [Test]
        public void InlineEditorAttribute_NoHeader()
        {
            var attr = new InlineEditorAttribute(false);
            Assert.IsFalse(attr.DrawHeader);
        }

        #endregion

        #region TableList Tests

        [Test]
        public void TableListAttribute_DefaultValues()
        {
            var attr = new TableListAttribute();
            Assert.IsTrue(attr.ShowPaging);
            Assert.AreEqual(20, attr.MaxItemsPerPage);
        }

        [Test]
        public void TableListAttribute_CustomValues()
        {
            var attr = new TableListAttribute(false, 10);
            Assert.IsFalse(attr.ShowPaging);
            Assert.AreEqual(10, attr.MaxItemsPerPage);
        }

        #endregion

        #region GUIColor Tests

        [Test]
        public void GUIColorAttribute_StoresColor()
        {
            var attr = new GUIColorAttribute(1f, 0.5f, 0f);
            Assert.AreEqual(1f, attr.R, 0.01f);
            Assert.AreEqual(0.5f, attr.G, 0.01f);
            Assert.AreEqual(0f, attr.B, 0.01f);
            Assert.AreEqual(1f, attr.A, 0.01f);
        }

        [Test]
        public void GUIColorAttribute_CustomAlpha()
        {
            var attr = new GUIColorAttribute(1f, 1f, 1f, 0.5f);
            Assert.AreEqual(0.5f, attr.A, 0.01f);
        }

        #endregion

        #region DisableInPlayMode / DisableInEditorMode Tests

        [Test]
        public void DisableInPlayModeAttribute_IsPropertyAttribute()
        {
            var attr = new DisableInPlayModeAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void DisableInEditorModeAttribute_IsPropertyAttribute()
        {
            var attr = new DisableInEditorModeAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
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
        public void ToggleButtonAttribute_BigParam()
        {
            var attr = new ToggleButtonAttribute(big: true);
            Assert.IsTrue(attr.Big);
            Assert.IsTrue(attr.FullWidth);
        }

        [Test]
        public void ToggleButtonAttribute_DefaultNotBig()
        {
            var attr = new ToggleButtonAttribute();
            Assert.IsFalse(attr.Big);
        }

        [Test]
        public void ToggleButtonAttribute_IsPropertyAttribute()
        {
            var attr = new ToggleButtonAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region TagField / LayerField / Indent Tests

        [Test]
        public void TagFieldAttribute_IsPropertyAttribute()
        {
            var attr = new TagFieldAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void LayerFieldAttribute_IsPropertyAttribute()
        {
            var attr = new LayerFieldAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void IndentAttribute_StoresLevel()
        {
            var attr = new IndentAttribute(3);
            Assert.AreEqual(3, attr.Level);
        }

        [Test]
        public void IndentAttribute_DefaultLevel()
        {
            var attr = new IndentAttribute();
            Assert.AreEqual(1, attr.Level);
        }

        #endregion

        #region FilePath Tests

        [Test]
        public void FilePathAttribute_DefaultValues()
        {
            var attr = new FilePathAttribute();
            Assert.IsNull(attr.Extensions);
            Assert.IsFalse(attr.AbsolutePath);
        }

        [Test]
        public void FilePathAttribute_CustomExtensions()
        {
            var attr = new FilePathAttribute { Extensions = "png,jpg" };
            Assert.AreEqual("png,jpg", attr.Extensions);
        }

        #endregion

        #region MultilineProperty Tests

        [Test]
        public void MultilinePropertyAttribute_StoresLines()
        {
            var attr = new MultilinePropertyAttribute(5);
            Assert.AreEqual(5, attr.Lines);
        }

        [Test]
        public void MultilinePropertyAttribute_DefaultLines()
        {
            var attr = new MultilinePropertyAttribute();
            Assert.AreEqual(3, attr.Lines);
        }

        #endregion

        #region AssetOnly / SceneObjectOnly Tests

        [Test]
        public void AssetOnlyAttribute_IsPropertyAttribute()
        {
            var attr = new AssetOnlyAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        [Test]
        public void SceneObjectOnlyAttribute_IsPropertyAttribute()
        {
            var attr = new SceneObjectOnlyAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region SearchableEnum Tests

        [Test]
        public void SearchableEnumAttribute_IsPropertyAttribute()
        {
            var attr = new SearchableEnumAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region SceneField Tests

        [Test]
        public void SceneFieldAttribute_IsPropertyAttribute()
        {
            var attr = new SceneFieldAttribute();
            Assert.IsInstanceOf<UnityEngine.PropertyAttribute>(attr);
        }

        #endregion

        #region ColorPalette Tests

        [Test]
        public void ColorPaletteAttribute_NamedPalette()
        {
            var attr = new ColorPaletteAttribute("Pastel");
            Assert.AreEqual("Pastel", attr.PaletteName);
            Assert.IsNull(attr.CustomColors);
        }

        [Test]
        public void ColorPaletteAttribute_CustomColors()
        {
            var attr = new ColorPaletteAttribute("#FF0000", "#00FF00");
            Assert.IsNull(attr.PaletteName);
            Assert.AreEqual(2, attr.CustomColors.Length);
        }

        [Test]
        public void ColorPaletteAttribute_DefaultPalette()
        {
            var attr = new ColorPaletteAttribute();
            Assert.AreEqual("Vivid", attr.PaletteName);
        }

        #endregion

        #region Conditional BoxGroup Tests

        [Test]
        public void BoxGroupAttribute_ShowIf_DefaultNull()
        {
            var attr = new BoxGroupAttribute("Test");
            Assert.IsNull(attr.ShowIf);
        }

        [Test]
        public void BoxGroupAttribute_ShowIf_CanBeSet()
        {
            var attr = new BoxGroupAttribute("Test") { ShowIf = "isVisible" };
            Assert.AreEqual("isVisible", attr.ShowIf);
        }

        #endregion

        #region ListDrawerSettings Tests

        [Test]
        public void ListDrawerSettingsAttribute_DefaultValues()
        {
            var attr = new ListDrawerSettingsAttribute();
            Assert.IsTrue(attr.ShowAddRemoveButtons);
            Assert.IsTrue(attr.Draggable);
            Assert.IsTrue(attr.ShowCount);
            Assert.AreEqual(0, attr.MinCount);
            Assert.AreEqual(0, attr.MaxCount);
            Assert.IsNull(attr.ElementLabel);
        }

        [Test]
        public void ListDrawerSettingsAttribute_CustomValues()
        {
            var attr = new ListDrawerSettingsAttribute
            {
                MinCount = 1, MaxCount = 10, ElementLabel = "Item $index",
                Draggable = false, ShowAddRemoveButtons = false
            };
            Assert.AreEqual(1, attr.MinCount);
            Assert.AreEqual(10, attr.MaxCount);
            Assert.AreEqual("Item $index", attr.ElementLabel);
            Assert.IsFalse(attr.Draggable);
        }

        #endregion

        #region OnInspectorInit / Dispose Tests

        [Test]
        public void OnInspectorInitAttribute_TargetsMethods()
        {
            var usage = typeof(OnInspectorInitAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsNotNull(usage);
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        [Test]
        public void OnInspectorDisposeAttribute_TargetsMethods()
        {
            var usage = typeof(OnInspectorDisposeAttribute).GetCustomAttribute<AttributeUsageAttribute>();
            Assert.IsNotNull(usage);
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        #endregion

        #region HideInPlayMode / HideInEditorMode Tests

        [Test]
        public void HideInPlayModeAttribute_CanBeCreated()
        {
            var attr = new HideInPlayModeAttribute();
            Assert.IsNotNull(attr);
        }

        [Test]
        public void HideInEditorModeAttribute_CanBeCreated()
        {
            var attr = new HideInEditorModeAttribute();
            Assert.IsNotNull(attr);
        }

        #endregion

        #region Wrap Tests

        [Test]
        public void WrapAttribute_StoresMinMax()
        {
            var attr = new WrapAttribute(0f, 360f);
            Assert.AreEqual(0f, attr.Min);
            Assert.AreEqual(360f, attr.Max);
        }

        [Test]
        public void WrapAttribute_StoresIntRange()
        {
            var attr = new WrapAttribute(0, 24);
            Assert.AreEqual(0f, attr.Min);
            Assert.AreEqual(24f, attr.Max);
        }

        #endregion

        #region DelayedProperty Tests

        [Test]
        public void DelayedPropertyAttribute_CanBeCreated()
        {
            var attr = new DelayedPropertyAttribute();
            Assert.IsNotNull(attr);
        }

        #endregion

        #region DisplayAsString Tests

        [Test]
        public void DisplayAsStringAttribute_DefaultValues()
        {
            var attr = new DisplayAsStringAttribute();
            Assert.IsFalse(attr.HideLabel);
            Assert.AreEqual(0, attr.FontSize);
        }

        [Test]
        public void DisplayAsStringAttribute_SupportsHideLabel()
        {
            var attr = new DisplayAsStringAttribute { HideLabel = true, FontSize = 14 };
            Assert.IsTrue(attr.HideLabel);
            Assert.AreEqual(14, attr.FontSize);
        }

        #endregion

        #region InlineButton Tests

        [Test]
        public void InlineButtonAttribute_StoresMethodName()
        {
            var attr = new InlineButtonAttribute("DoSomething");
            Assert.AreEqual("DoSomething", attr.MethodName);
            Assert.IsNull(attr.Label);
            Assert.AreEqual(ButtonMode.AlwaysEnabled, attr.Mode);
        }

        [Test]
        public void InlineButtonAttribute_StoresCustomLabel()
        {
            var attr = new InlineButtonAttribute("Randomize", "↻");
            Assert.AreEqual("Randomize", attr.MethodName);
            Assert.AreEqual("↻", attr.Label);
        }

        [Test]
        public void InlineButtonAttribute_SupportsButtonMode()
        {
            var attr = new InlineButtonAttribute("Test") { Mode = ButtonMode.EnabledInPlayMode };
            Assert.AreEqual(ButtonMode.EnabledInPlayMode, attr.Mode);
        }

        [Test]
        public void InlineButtonAttribute_SupportsCustomWidth()
        {
            var attr = new InlineButtonAttribute("Test") { Width = 80f };
            Assert.AreEqual(80f, attr.Width);
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
