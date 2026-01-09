using System;
using System.Text;

namespace ConsoleUtils.Output;

/// <summary>
/// Represents console output that can be printed to a console.
/// </summary>
public abstract partial class ConsoleOutput
{

    /// <summary>
    /// Prints the console output to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to print to.</param>
    public abstract void Print(StringBuilder stringBuilder);


    /// <summary>
    /// Stylizes the specified <see cref="ConsoleOutput"/> with the given styles.
    /// </summary>
    /// <param name="styles">The styles to apply.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> instance with the applied styles.</returns>
    public ConsoleOutput Stylize(HashSet<Style> styles) =>
        new OutputStyle(this, styles);


    /// <summary>
    /// Stylizes the specified <see cref="ConsoleOutput"/> with the given styles.
    /// </summary>
    /// <param name="styles">The styles to apply.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> instance with the applied styles.</returns>
    public ConsoleOutput Stylize(params IEnumerable<Style> styles) =>
        new OutputStyle(this, [.. styles]);


    /// <summary>
    /// Concatenates the specified <see cref="ConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="outputs">The other console outputs to concatenate.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> that represents the concatenated output.</returns>
    public ConsoleOutput Concat(params IEnumerable<ConsoleOutput> outputs) =>
        Concat(resetStylesOnEnd: false, outputs);


    /// <summary>
    /// Concatenates the specified <see cref="ConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="resetStylesOnEnd">Whether to reset styles at the end of the concatenated output.</param>
    /// <param name="outputs">The other console outputs to concatenate.</param>
    /// <returns>A new <see cref="ConsoleOutput"/> that represents the concatenated output.</returns>
    public ConsoleOutput Concat(bool resetStylesOnEnd, params IEnumerable<ConsoleOutput> outputs) =>
        new MultiOutput([this, .. outputs], resetStylesOnEnd);
}
