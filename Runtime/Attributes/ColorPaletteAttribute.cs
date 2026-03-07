using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays a Color field with a row of preset swatches above it.
    /// Use PaletteType to choose built-in palettes, or provide custom hex colors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ColorPaletteAttribute : PropertyAttribute
    {
        public string PaletteName { get; private set; }
        public string[] CustomColors { get; private set; }

        /// <summary>Use a named built-in palette: "Pastel", "Vivid", "Greyscale", "Warm", "Cool".</summary>
        public ColorPaletteAttribute(string paletteName = "Vivid")
        {
            PaletteName = paletteName;
            CustomColors = null;
        }

        /// <summary>Provide custom hex colors (e.g. "#FF0000", "#00FF00").</summary>
        public ColorPaletteAttribute(params string[] hexColors)
        {
            PaletteName = null;
            CustomColors = hexColors;
        }
    }
}
