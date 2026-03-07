# Changelog

## [1.6.2] - 2026-03-07

### Added
- `[InlineButton(methodName, label)]` — draws a small button on the same line as a property field, invoking the specified method. Supports `ButtonMode`, custom `Width`, and `AllowMultiple`.

## [1.6.1] - 2026-03-07

### Fixed
- `FilePathAttribute.Extensions` changed to read-write property (fixes CS0200/CS0617 compile errors when using named attribute syntax)
- Added missing constructor overloads `(string, bool)` and `(string, float)` to `ShowIfAttribute`, `HideIfAttribute`, and `EnableIfAttribute`
- `ConditionResolver.CompareValues` now explicitly handles `bool` and `float` comparisons (float uses epsilon tolerance)

## [1.6.0] - 2026-03-05

### Added
- **Batch 1 — Simple Attributes:**
  - `[TagField]` — tag dropdown selector for string fields
  - `[LayerField]` — layer dropdown selector for int fields
  - `[Indent(level)]` — adjusts inspector indentation level
  - `[FilePath(Extensions, AbsolutePath)]` — file browser with extension filter
  - `[MultilineProperty(lines)]` — multi-line text area with configurable height
  - `[AssetOnly]` — restricts ObjectField to project assets only
  - `[SceneObjectOnly]` — restricts ObjectField to scene objects only
- **Batch 2 — Medium Attributes:**
  - `[SearchableEnum]` — searchable popup window for large enums
  - `[SceneField]` — scene asset picker with Build Settings validation
  - `[ColorPalette("name")]` — color picker with preset swatches (Vivid, Pastel, Greyscale, Warm, Cool, or custom hex)
  - `[BoxGroup("name", ShowIf = "condition")]` — conditional box group visibility
- **Batch 3 — Complex Attributes:**
  - `[ListDrawerSettings]` — customize list rendering (Draggable, ShowCount, MinCount, MaxCount, ElementLabel)
  - `[Button]` with parameters — methods with editable params displayed above button
  - `[OnInspectorInit]` — callback when inspector opens (OnEnable)
  - `[OnInspectorDispose]` — callback when inspector closes (OnDisable)

## [1.5.0] - 2026-03-05

### Added
- `[Button(HorizontalGroup = "name")]` — buttons sharing the same group are drawn side by side
- `[ShowInInspector(runtimeOnly: true)]` — hides the entry in Editor mode, visible only in Play mode
- ConditionResolver now walks the full type hierarchy (fixes warnings for inherited fields)

## [1.4.0] - 2026-03-05

### Added
- SectionHeader optional custom color parameter (`[SectionHeader("Title", 1f, 0.3f, 0.3f)]`)
- SectionHeader color support for all styles (text + lines/borders)
- GUIColor `Color` property for convenient access

### Changed
- SectionHeader default style changed to CenterLine
- SectionHeader Box style now has full 4-sided borders (top, bottom, left, right)
- SectionHeader font size increased to 13px (matches BoxGroup)
- FolderPath now opens native OS folder browser dialog
- FolderPath supports AbsolutePath and RequireExistingPath parameters

## [1.3.0] - 2026-03-05

### Added
- SectionHeader 4 styles: Line (default), CenterLine, Box, Clean
- BoxGroup foldable option (`[BoxGroup("name", foldable: true)]`)
- ToggleButton big param for double-height buttons (`[ToggleButton(big: true)]`)
- PreviewField enhanced: metadata display (dimensions, file size, asset path), native typed object picker, drag & drop, clear button, ping button. Supports Texture2D, Sprite, Mesh, GameObject

### Changed
- BoxGroup title font size increased (13px) for better visibility
- BoxGroup foldout style matches helpBox background (no dark rect)

## [1.2.0] - 2026-03-04

### Added
- ToggleButton green pastel ON (0.6, 0.9, 0.65) / red pastel OFF (0.9, 0.6, 0.6) colors
- ShowIf/HideIf/EnableIf evaluation at editor level (compatible with HorizontalGroup + ToggleButton)
- Stable sort for PropertyOrder (preserves declaration order on ties)
- SectionHeader + HorizontalGroup compatibility (SuppressNextDraw mechanism)

## [1.1.0] - 2026-03-03

### Added
- 20 Odin-like inspector attributes: ShowIf, HideIf, EnableIf, BoxGroup, FoldoutGroup, TabGroup, HorizontalGroup, PropertyOrder, PropertySpace, SectionHeader, ProgressBar, MinMaxSlider, SuffixLabel, EnumToggleButtons, GUIColor, DisableInPlayMode, DisableInEditorMode, InlineButton, ValueDropdown, OnValueChanged, ValidateInput, InfoBox, PreviewField, InlineEditor, TableList, ShowInInspector, ToggleButton, FreeRange, Required
- ABGroupedEditor base editor for all MonoBehaviour/ScriptableObject
- Unit tests for all attributes
- Showcase script (UtilsInspectorShowcase)

## [1.0.0] - 2026-03-02

### Added
- Initial release extracted from AnkleBreaker-Utils monolithic package
- 13 custom inspector attributes (HideInNormalInspector, ShowIf, HideIf, ReadonlyName, etc.)
- 12 property drawers for all attributes
- AB_SerializedDictionary serialized class
- AutoCompleteText system
- OdinStub for Odin Inspector compatibility
- ABEditor base editor class