using System;

namespace ConsoleUtils.Output;

internal sealed class OutputStyle(
    IConsoleOutput output,
    HashSet<Style> styles) : OutputModBase(
        output,
        Prefix(styles),
        Suffix(styles))
{
    private static string Prefix(HashSet<Style> styles) => StylesString(styles, style => style.Code);

    private static string Suffix(HashSet<Style> styles) =>
        StylesString(styles, style => style.ResetCode);

    private static string StylesString(HashSet<Style> styles, Func<Style, int> codeSelector) =>
        styles.Count > 0
            ? $"\e[{string.Join(';', styles.Select(codeSelector))}m"
            : "";
}

public readonly record struct Style(int Code, int ResetCode)
{
    private const int DEFAULT_COLOR_CODE = 39;
    public static readonly Style Bold = new(1, 22);
    public static readonly Style Dim = new(2, 22);
    public static readonly Style Italic = new(3, 23);
    public static readonly Style Underline = new(4, 24);
    public static readonly Style DoubleUnderline = new(21, 24);
    public static readonly Style BLinking = new(5, 25);
    public static readonly Style Inverse = new(7, 27);
    public static readonly Style Hidden = new(8, 28);
    public static readonly Style StrikeThrough = new(9, 29);
    public static readonly Color Black = new(30);
    public static readonly Color Red = new(31);
    public static readonly Color Green = new(32);
    public static readonly Color Yellow = new(33);
    public static readonly Color Blue = new(34);
    public static readonly Color Magenta = new(35);
    public static readonly Color Cyan = new(36);
    public static readonly Color White = new(37);
    public static readonly Color DefaultColor = new(DEFAULT_COLOR_CODE);

    public readonly record struct Color
    {
        private const int BACKGROUND_CODE_DIFF = 10;

        public Style Forward { get; init; }
        public Style Background { get; init; }

        public Color(int colorCode)
        {
            Forward = new Style(colorCode, DEFAULT_COLOR_CODE);
            Background = new Style(
                Forward.Code + BACKGROUND_CODE_DIFF,
                Forward.ResetCode + BACKGROUND_CODE_DIFF);
        }
    }
}
