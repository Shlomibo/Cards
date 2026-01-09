using ConsoleUtils.Output;

namespace ConsoleUtils;

/// <summary>
/// Represents a console interface for reading and writing output.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Reads a line of input from the console.
    /// </summary>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The task result contains the read line.
    /// </returns>
    Task<string?> ReadLine(CancellationToken cancellation);

    /// <summary>
    /// Writes an empty line to the console.
    /// </summary>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteLine(CancellationToken cancellation);

    /// <summary>
    /// Writes the specified console output to the console, followed by a new line.
    /// </summary>
    /// <param name="output">The console output to write.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteLine(ConsoleOutput output, CancellationToken cancellation);

    /// <summary>
    /// Clears the console.
    /// </summary>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous clear operation.</returns>
    Task Clear(CancellationToken cancellation);
}

/// <summary>
/// Provides extension methods for the <see cref="IConsole"/> interface.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Writes the specified interpolated console output to the console, followed by a new line.
    /// </summary>
    /// <param name="console">The console to write to.</param>
    /// <param name="output">The interpolated console output to write.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static Task WriteLine(
        this IConsole console,
        InterpolatedConsoleOutput output,
        CancellationToken cancellation)
        =>
        console.WriteLine((ConsoleOutput)output, cancellation);
}
