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
            { DefinedColors.primary, "#F7F7F7" },
            { DefinedColors.secondary, "#000000" },
            { DefinedColors.success, "#2C3930" },
            { DefinedColors.danger, "#56021F" },
            { DefinedColors.warning, "#FFB22C" },
            { DefinedColors.info, "#00879E" },
            { DefinedColors.accent, "#AA60C8" },
            { DefinedColors.neutral, "#FFDFEF" }

        };

        public static Color GetColor(DefinedColors color)
        {
            return ColorTranslator.FromHtml(UIColors[color]);
        }
    }
}