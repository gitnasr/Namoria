using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Client.UI
{
    public enum DefinedColors
    {
        primary,
        secondary,
        success,
        danger,
        warning,
        info,
        accent,
        neutral
    }

    public static class Colors
    {
        private static readonly Dictionary<DefinedColors, string> UIColors = new Dictionary<DefinedColors, string>
        {
            { DefinedColors.primary, "#09122C" },
            { DefinedColors.secondary, "#872341" },
            { DefinedColors.success, "#229954" },
            { DefinedColors.danger, "#E17564" },
            { DefinedColors.warning, "#FFC107" },
            { DefinedColors.info, "#17A2B8" },
                        { DefinedColors.accent, "#4B0082" },   
            { DefinedColors.neutral, "#2F4F4F" }   

        };

        public static Color GetColor(DefinedColors color)
        {
            return ColorTranslator.FromHtml(UIColors[color]);
        }
    }
}