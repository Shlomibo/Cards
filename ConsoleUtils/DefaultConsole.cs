using System;
using System.Text;
using ConsoleUtils.Output;
using Nito.Disposables;

namespace ConsoleUtils;

public sealed class DefaultConsole : IConsole
{
    public async Task<string?> ReadLine(CancellationToken cancellation) =>
        await Console.In.ReadLineAsync(cancellation);

    public Task WriteLine(CancellationToken cancellation) =>
        Console.Out.WriteLineAsync();
    public Task WriteLine<T>(T? content, CancellationToken cancellation) =>
        content switch
        {
            null => WriteLine(cancellation),
            char ch => Console.Out.WriteLineAsync(ch),
            char[] buffer => Console.Out.WriteLineAsync(buffer),
            StringBuilder builder => Console.Out.WriteLineAsync(builder, cancellation),
            ReadOnlyMemory<char> memory => Console.Out.WriteLineAsync(memory, cancellation),
            _ => Console.Out.WriteLineAsync(content?.ToString()),
        };

    public async Task Clear(CancellationToken cancellation) =>
        Console.Clear();

    public Task WriteLine(string line, CancellationToken cancellation) =>
        Console.Out.WriteLineAsync(line);

    public Task WriteLine(IConsoleOutput output, CancellationToken cancellation)
    {
        StringBuilder outputBuilder = new();
        output.Print(outputBuilder);

        return Console.Out.WriteLineAsync(outputBuilder, cancellation);
    }

    public Task WriteLine(InterpolatedConsoleOutput output, CancellationToken cancellation) =>
        WriteLine((IConsoleOutput)output, cancellation);
}
