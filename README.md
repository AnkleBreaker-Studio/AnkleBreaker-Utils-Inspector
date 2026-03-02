# AnkleBreaker Utils - Inspector

Custom inspector attributes and property drawers for Unity Editor.

## Installation

Add via Unity Package Manager using Git URL:
```
https://github.com/AnkleBreaker-Studio/utils-inspector.git#Release
```

## Attributes

| Attribute | Description |
|-----------|-------------|
| `[HideInNormalInspector]` | Hides field in default inspector, visible in custom inspectors |
| `[ShowIf("field")]` | Shows field conditionally based on a boolean |
| `[HideIf("field")]` | Hides field conditionally based on a boolean |
| `[ReadonlyName]` | Makes field read-only in inspector |
| `[ReadOnlyEnumDrawer]` | Read-only enum flags display |
| `[LabelText("text")]` | Custom label for inspector field |
| `[HideVariableName]` | Hides the variable name in inspector |
| `[HelpBox("msg", type)]` | Displays a help box in inspector |
| `[FolderPath]` | Folder picker with browse button |
| `[EnumDescription]` | Enum with description tooltips |
| `[AutoCompleteText]` | Text field with autocomplete suggestions |
| `[ABToolTip("tip")]` | Enhanced tooltip attribute |
| `[Button]` | Displays a method as a button in inspector |

## Serialized Classes

- `AB_SerializedDictionary<TKey, TValue>` — Serializable dictionary for Unity inspector

## Editor Utilities

- `ABEditor` — Base editor class with utility methods

## Requirements

- Unity 2022.3 LTS or later

## License

See [LICENSE.md](LICENSE.md)