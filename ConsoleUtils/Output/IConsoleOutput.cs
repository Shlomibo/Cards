using System;
using System.Text;

namespace ConsoleUtils.Output;

/// <summary>
/// Represents console output that can be printed to a console.
/// </summary>
public interface IConsoleOutput
{
    /// <summary>
    /// Prints the console output to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to print to.</param>
    void Print(StringBuilder stringBuilder);

    /// <summary>
    /// Combines two <see cref="IConsoleOutput"/> instances into a single instance.
    /// </summary>
    /// <param name="left">The left <see cref="IConsoleOutput"/>.</param>
    /// <param name="right">The right <see cref="IConsoleOutput"/>.</param>
    /// <returns>A new <see cref="IConsoleOutput"/> that represents the combined output.</returns>
    static virtual IConsoleOutput operator +(IConsoleOutput left, IConsoleOutput right) =>
        new MultiOutput([left, right]);
}
