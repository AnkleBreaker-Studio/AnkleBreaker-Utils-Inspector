using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ColorPaletteAttribute))]
    public class ColorPaletteDrawer : PropertyDrawer
    {
        private static readonly Color[][] BuiltInPalettes = new Color[][]
        {
            // Vivid
            new[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black },
            // Pastel
            new[] { c(0.98f,0.76f,0.76f), c(0.76f,0.98f,0.76f), c(0.76f,0.76f,0.98f), c(0.98f,0.98f,0.76f), c(0.76f,0.98f,0.98f), c(0.98f,0.76f,0.98f) },
            // Greyscale
            new[] { Color.black, c(0.2f,0.2f,0.2f), c(0.4f,0.4f,0.4f), c(0.6f,0.6f,0.6f), c(0.8f,0.8f,0.8f), Color.white },
            // Warm
            new[] { c(1f,0.2f,0f), c(1f,0.5f,0f), c(1f,0.8f,0f), c(1f,1f,0f), c(0.8f,0.2f,0.1f), c(0.6f,0.1f,0f) },
            // Cool
            new[] { c(0f,0.4f,1f), c(0f,0.7f,1f), c(0f,1f,1f), c(0.2f,0.8f,0.6f), c(0.4f,0.2f,0.8f), c(0.1f,0.1f,0.6f) }
        };

        private static readonly string[] PaletteNames = { "Vivid", "Pastel", "Greyscale", "Warm", "Cool" };

        private static Color c(float r, float g, float b) => new Color(r, g, b, 1f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Color)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            ColorPaletteAttribute attr = (ColorPaletteAttribute)attribute;
            Color[] palette = GetPalette(attr);

            EditorGUI.BeginProperty(position, label, property);

            // Label
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // Swatch row
            float swatchY = position.y;
            float swatchStartX = position.x + EditorGUIUtility.labelWidth + 2f;
            float availableWidth = position.width - EditorGUIUtility.labelWidth - 2f;
            float swatchSize = Mathf.Min(20f, availableWidth / palette.Length - 2f);

            for (int i = 0; i < palette.Length; i++)
            {
                Rect swatchRect = new Rect(swatchStartX + i * (swatchSize + 2f), swatchY, swatchSize, swatchSize);

                // Draw border if selected
                if (ColorsEqual(property.colorValue, palette[i]))
                {
                    Rect borderRect = new Rect(swatchRect.x - 1, swatchRect.y - 1, swatchRect.width + 2, swatchRect.height + 2);
                    EditorGUI.DrawRect(borderRect, Color.white);
                }

                EditorGUI.DrawRect(swatchRect, palette[i]);

                if (Event.current.type == EventType.MouseDown && swatchRect.Contains(Event.current.mousePosition))
                {
                    property.colorValue = palette[i];
                    Event.current.Use();
                }
            }

            // Color field below swatches
            float colorFieldY = position.y + swatchSize + 4f;
            Rect colorRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2f, colorFieldY, availableWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            Color newColor = EditorGUI.ColorField(colorRect, GUIContent.none, property.colorValue);
            if (EditorGUI.EndChangeCheck())
                property.colorValue = newColor;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Color)
                return EditorGUIUtility.singleLineHeight;
            return 20f + 4f + EditorGUIUtility.singleLineHeight; // swatches + gap + color field
        }

        private static Color[] GetPalette(ColorPaletteAttribute attr)
        {
            if (attr.CustomColors != null && attr.CustomColors.Length > 0)
            {
                Color[] custom = new Color[attr.CustomColors.Length];
                for (int i = 0; i < attr.CustomColors.Length; i++)
                {
                    if (!ColorUtility.TryParseHtmlString(attr.CustomColors[i], out custom[i]))
                        custom[i] = Color.magenta;
                }
                return custom;
            }

            if (!string.IsNullOrEmpty(attr.PaletteName))
            {
                for (int i = 0; i < PaletteNames.Length; i++)
                {
                    if (string.Equals(PaletteNames[i], attr.PaletteName, System.StringComparison.OrdinalIgnoreCase))
                        return BuiltInPalettes[i];
                }
            }

            return BuiltInPalettes[0]; // default Vivid
        }

        private static bool ColorsEqual(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.01f && Mathf.Abs(a.g - b.g) < 0.01f &&
                   Mathf.Abs(a.b - b.b) < 0.01f && Mathf.Abs(a.a - b.a) < 0.01f;
        }
    }
}
