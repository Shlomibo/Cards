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
    /// Writes the specified string to the console, followed by a new line.
    /// </summary>
    /// <param name="line">The string to write.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteLine(string line, CancellationToken cancellation);

    /// <summary>
    /// Writes the specified console output to the console, followed by a new line.
    /// </summary>
    /// <param name="output">The console output to write.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteLine(IConsoleOutput output, CancellationToken cancellation);

    /// <summary>
    /// Writes the specified interpolated console output to the console, followed by a new line.
    /// </summary>
    /// <param name="output">The interpolated console output to write.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteLine(InterpolatedConsoleOutput output, CancellationToken cancellation);

    /// <summary>
    /// Clears the console.
    /// </summary>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous clear operation.</returns>
    Task Clear(CancellationToken cancellation);
}
