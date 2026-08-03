# Changelog

## [1.7.0] - 2026-08-03

### Added
- `AB_ObjectCompat` (runtime) — the shared Unity object-identity fork, so every AnkleBreaker package resolves an object id from one place instead of carrying its own copy
  - `StableId(Object)` returns the identity at full width on every supported editor, from 2022.3 up
  - `StableIdAsInt(Object)` folds it into an `int` for callers that cannot widen — a value packed into a fixed 32-bit field, or an existing public API that returns an `int`
- `AB_EditorObjectCompat` (editor) — `HoldsALostReference(SerializedProperty)`, the same fork for telling a reference whose asset is gone from an empty one

The fork exists because neither spelling covers the supported range: `Object.GetInstanceID()` is clean through 6000.3, obsolete in 6000.4 and a compile error in 6000.5, while its `EntityId` replacement does not exist before 6000.4. Identities widen to eight bytes in 6000.5, so `StableId` returns a `long` and never the truncating implicit `int` conversion.

### Fixed
- `AB_SerializedDictionaryPropertyDrawer` no longer calls `Object.GetInstanceID()`, a compile error (CS0619) on Unity 6000.5 that broke the whole editor assembly. Its three call sites route through `AB_ObjectCompat.StableId`

## [1.6.11] - 2026-04-10

### Fixed
- Compilation error with InlineButtonDrawer when Odin Inspector is installed

## [1.6.10] - 2026-04-01

### Fixed
- `AB_SerializedDictionaryPropertyDrawer` now resolves actual TKey/TValue types for custom classes — ObjectField filters by real type instead of generic `Object`, EnumPopup shows enum names, column headers display actual types
- Fixed `ArgumentNullException` crash when clicking "+" on dictionaries with reference-type keys — CreateStaging now uses pure reflection instead of mutating serialized arrays (prevents `OnAfterDeserialize` from firing with null keys)

## [1.6.9] - 2026-04-01

### Added
- Pagination for `AB_SerializedDictionaryPropertyDrawer` (12 items per page, Prev/Next navigation)
- Right-click context menu on dictionary: Copy, Paste, Clear Dictionary
  - Copy serializes all entries as JSON to system clipboard
  - Paste deserializes JSON and adds non-duplicate entries
  - Clear removes all entries

### Changed
- Page size set to 12 items per page

## [1.6.8] - 2026-03-31

### Added
- `AB_SerializedDictionaryPropertyDrawer` — custom PropertyDrawer for `AB_SerializedDictionary<K,V>`
  - Foldout header with item count and "+" button
  - Staging area (Key/Value fields) with "Add" button to commit new entries
  - Duplicate key detection: "Add" button greyed out with warning message when key already exists
  - Table view with "Key" / "Value" column headers, alternating row colors, per-row "×" remove button
  - Staging values stored in memory only — never interferes with dictionary serialization round-trip
  - Supports dark and light editor themes

## [1.6.7] - 2026-03-30

### Fixed
- `FoldoutGroupAttribute.Style` changed to read-write property (fixes CS0617 when using named attribute syntax `Style = FoldoutGroupStyle.Line`)
- `ShowInInspectorAttribute.RuntimeOnly` changed to read-write property (fixes CS0617 when using named attribute syntax `RuntimeOnly = true`)

## [1.6.5] - 2026-03-08

### Removed
- `FoldoutGroupStyle.Box` — redundant with `[BoxGroup("name", foldable: true)]`; enum now has 4 styles: Default, Line, CenterLine, Clean

## [1.6.4] - 2026-03-08

### Added
- `[FoldoutGroup]` now supports visual styles via `FoldoutGroupStyle`: Default, Line, CenterLine, Clean — with optional custom color (same pattern as SectionHeader)

### Fixed
- `InlineButtonAttribute` missing `using UnityEngine` causing CS0246 compile error

## [1.6.3] - 2026-03-07

### Added
- `[HideInPlayMode]` — hides a field during Play Mode
- `[HideInEditorMode]` — hides a field in Editor Mode (only visible during Play Mode)
- `[Wrap(min, max)]` — cyclic value wrapping for int and float fields (e.g., 0–360 for angles)
- `[DelayedProperty]` — value only applied on Enter or focus loss (string, int, float)
- `[DisplayAsString]` — displays field value as read-only label, supports `HideLabel` and `FontSize`

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
- 20 inspector attributes: ShowIf, HideIf, EnableIf, BoxGroup, FoldoutGroup, TabGroup, HorizontalGroup, PropertyOrder, PropertySpace, SectionHeader, ProgressBar, MinMaxSlider, SuffixLabel, EnumToggleButtons, GUIColor, DisableInPlayMode, DisableInEditorMode, InlineButton, ValueDropdown, OnValueChanged, ValidateInput, InfoBox, PreviewField, InlineEditor, TableList, ShowInInspector, ToggleButton, FreeRange, Required
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
- ABEditor base editor class