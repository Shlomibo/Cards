using System;

namespace ConsoleUtils.Output;

/// <summary>
/// Provides utility methods for creating and manipulating console output.
/// </summary>
public static class ConsoleOutput
{
    /// <summary>
    /// Creates a new <see cref="IConsoleOutput"/> from the specified string.
    /// </summary>
    /// <param name="output">The string to create the console output from.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> instance.</returns>ary>
    public static IConsoleOutput FromString(string? output) =>
        new StringConsoleOutput(output);

    /// <summary>
    /// Stylizes the specified <see cref="IConsoleOutput"/> with the given styles.
    /// </summary>
    /// <param name="output">The console output to stylize.</param>
    /// <param name="styles">The styles to apply.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> instance with the applied styles.</returns>
    public static IConsoleOutput Stylize(this IConsoleOutput output, HashSet<Style> styles) =>
        new OutputStyle(output, styles);

    /// <summary>
    /// Stylizes the specified <see cref="IConsoleOutput"/> with the given styles.
    /// </summary>
    /// <param name="output">The console output to stylize.</param>
    /// <param name="styles">The styles to apply.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> instance with the applied styles.</returns>
    public static IConsoleOutput Stylize(this IConsoleOutput output, params IEnumerable<Style> styles) =>
        new OutputStyle(output, [.. styles]);

    /// <summary>
    /// Concatenates the specified <see cref="IConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="output">The first console output.</param>
    /// <param name="outputs">The other console outputs to concatenate.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> that represents the concatenated output.</returns>
    public static IConsoleOutput Concat(this IConsoleOutput output, params IEnumerable<IConsoleOutput> outputs) =>
        output.Concat(resetStylesOnEnd: false, outputs);

    /// <summary>
    /// Concatenates the specified <see cref="IConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="output">The first console output.</param>
    /// <param name="resetStylesOnEnd">Whether to reset styles at the end of the concatenated output.</param>
    /// <param name="outputs">The other console outputs to concatenate.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> that represents the concatenated output.</returns>
    public static IConsoleOutput Concat(this IConsoleOutput output, bool resetStylesOnEnd, params IEnumerable<IConsoleOutput> outputs) =>
        new MultiOutput([output, .. outputs], resetStylesOnEnd);

    /// <summary>
    /// Create an <see cref="IConsoleOutput"/> from a concatenated string such as
    /// <c>$"Value: {someValue}, Content: {someConsoleOutput}."</c>
    /// <example>
    /// int i = 5;
    /// string str = "some string"
    /// IConsoleOutput output = ConsoleOutput.FromString("important")
    ///     .Stylize(Style.Italic, Style.Bold, Style.Underline);
    ///
    /// IConsoleOutput interpolated = ConsoleOutput.Interpolate($"{i:00}: {someString}\n\t{output}");
    /// // Results with:
    /// // 05: some string
    /// //     \e[1;3;4mimportant\e[22;23;24m
    /// </example>
    /// </summary>
    /// <param name="interpolatedOutput">The interpolated console output.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> instance.</returns>
    public static IConsoleOutput Interpolate(InterpolatedConsoleOutput interpolatedOutput) =>
        interpolatedOutput;
}


