using System;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleUtils.Output;

/// <summary>
/// Provides utility methods for creating and manipulating console output.
/// </summary>
public abstract partial class ConsoleOutput
{
    /// <summary>
    /// Creates a new <see cref="ConsoleOutput"/> from the specified string.
    /// </summary>
    /// <param name="output">The string to create the console output from.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> instance.</returns>ary>
    public static ConsoleOutput FromString(string? output) =>
        FromValue(output);

    /// <summary>
    /// Creates a new <see cref="ConsoleOutput"/> from the specified value with optional formatting.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to create the console output from.</param>
    /// <param name="format">An optional format string.</param>
    /// <param name="alignment">An optional alignment value.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> instance.</returns>
    public static ConsoleOutput FromValue<T>(T? value, string? format = null, int? alignment = null) =>
        new FormattedValueOutput<T>(value, format, alignment);


    /// <summary>
    /// Concatenates the specified <see cref="ConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="outputs">The other console outputs to concatenate.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> that represents the concatenated output.</returns>
    public static ConsoleOutput ConcatAll(params IEnumerable<ConsoleOutput>? outputs) =>
        outputs?.FirstOrDefault()?.Concat(outputs.Skip(1)) ?? Empty();

    /// <summary>
    /// Creates an empty <see cref="ConsoleOutput"/>.
    /// </summary>
    /// <returns>A new <see cref="ConsoleOutput"/> instance representing empty output.</returns>
    public static ConsoleOutput Empty() => EmptyOutput.Instance;

    /// <summary>
    /// Create an <see cref="ConsoleOutput"/> from a concatenated string such as
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
    /// <returns>A new <see cref="ConsoleOutput"/> instance.</returns>
    public static ConsoleOutput Interpolate(InterpolatedConsoleOutput interpolatedOutput) =>
        interpolatedOutput;

    /// <summary>
    /// Combines two <see cref="ConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="left">The left <see cref="ConsoleOutput"/>.</param>
    /// <param name="right">The right <see cref="ConsoleOutput"/>.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> that represents the combined output.</returns>
    public static ConsoleOutput operator +(ConsoleOutput left, ConsoleOutput right) =>
        new MultiOutput([left, right]);

    /// <summary>
    /// Implicitly converts a string to a <see cref="ConsoleOutput"/>.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> instance.</returns>
    public static implicit operator ConsoleOutput(string? value) => FromValue(value);
}


