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

/// <summary>
/// Represents a console output style.
/// </summary>
public readonly record struct Style(int Code, int ResetCode)
{
    private const int DEFAULT_COLOR_CODE = 39;

    /// <summary>
    /// The bold style.
    /// </summary>
    public static readonly Style Bold = new(1, 22);

    /// <summary>
    /// The bright style (alias for bold).
    /// </summary>
    public static readonly Style Bright = Bold;

    /// <summary>
    /// The dim style.
    /// </summary>
    public static readonly Style Dim = new(2, 22);

    /// <summary>
    /// The dark style (alias for dim).
    /// </summary>
    public static readonly Style Dark = Dim;

    /// <summary>
    /// The italic style.
    /// </summary>
    public static readonly Style Italic = new(3, 23);

    /// <summary>
    /// The underline style.
    /// </summary>
    public static readonly Style Underline = new(4, 24);

    /// <summary>
    /// The double underline style.
    /// </summary>
    public static readonly Style DoubleUnderline = new(21, 24);

    /// <summary>
    /// The blinking style.
    /// </summary>
    public static readonly Style BLinking = new(5, 25);

    /// <summary>>
    /// The inverse style.
    /// </summary>
    public static readonly Style Inverse = new(7, 27);

    /// <summary>>
    /// The hidden style.
    /// </summary>
    public static readonly Style Hidden = new(8, 28);

    /// <summary>>
    /// The strike-through style.
    /// </summary>
    public static readonly Style StrikeThrough = new(9, 29);

    /// <summary>>
    /// The black color.
    /// </summary>
    public static readonly Color Black = new(30);

    /// <summary>>
    /// The red color.
    /// </summary>
    public static readonly Color Red = new(31);

    /// <summary>>
    /// The green color.
    /// </summary>
    public static readonly Color Green = new(32);

    /// <summary>>
    /// The yellow color.
    /// </summary>
    public static readonly Color Yellow = new(33);

    /// <summary>>
    /// The blue color.
    /// </summary>
    public static readonly Color Blue = new(34);

    /// <summary>>
    /// The magenta color.
    /// </summary>
    public static readonly Color Magenta = new(35);

    /// <summary>
    /// The cyan color.
    /// </summary>
    public static readonly Color Cyan = new(36);

    /// <summary>
    /// The white color.
    /// </summary>
    public static readonly Color White = new(37);

    /// <summary>
    /// The default console color.
    /// </summary>
    public static readonly Color DefaultColor = new(DEFAULT_COLOR_CODE);

    /// <summary>
    /// Represents a console color with forward and background styles.
    /// </summary>
    public readonly record struct Color
    {
        private const int BACKGROUND_CODE_DIFF = 10;

        /// <summary>
        /// The foreground style.
        /// </summary>
        public Style Forward { get; init; }

        /// <summary>
        /// The background style.
        /// </summary>
        public Style Background { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct with the specified color code.
        /// </summary>
        /// <param name="colorCode">The ANSI color code for the foreground color.</param>
        public Color(int colorCode)
        {
            Forward = new Style(colorCode, DEFAULT_COLOR_CODE);
            Background = new Style(
                Forward.Code + BACKGROUND_CODE_DIFF,
                Forward.ResetCode + BACKGROUND_CODE_DIFF);
        }
    }
}
