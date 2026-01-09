using System;
using System.Text;
using ConsoleUtils.Output;

namespace ConsoleUtils;

/// <inheritdoc cref="IConsole"/>
public sealed class DefaultConsole : IConsole
{
    /// <inheritdoc/>
    public async Task<string?> ReadLine(CancellationToken cancellation) =>
        await Console.In.ReadLineAsync(cancellation);

    /// <inheritdoc/>
    public Task WriteLine(CancellationToken cancellation) =>
        Console.Out.WriteLineAsync();

    /// <inheritdoc/>
    public async Task Clear(CancellationToken cancellation) =>
        Console.Clear();

    /// <inheritdoc/>
    public Task WriteLine(ConsoleOutput output, CancellationToken cancellation)
    {
        StringBuilder outputBuilder = new();
        output.Print(outputBuilder);

        return Console.Out.WriteLineAsync(outputBuilder, cancellation);
    }
}
