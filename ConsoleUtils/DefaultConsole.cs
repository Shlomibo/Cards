using System;
using System.Text;
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

    public async Task WriteLine<T>(T? content, Color color, CancellationToken cancellation)
    {
        using (SetColor(color))
        {
            await WriteLine(content, cancellation);
        }
    }

    public async Task Clear(CancellationToken cancellation) =>
        Console.Clear();

    private static IDisposable SetColor(Color color)
    {
        Color current = new(Console.ForegroundColor, Console.BackgroundColor);

        Console.ForegroundColor = color.Foreground;

        if (color.Background.HasValue)
        {
            Console.BackgroundColor = color.Background.Value;
        }

        return new Disposable(() =>
        {
            Console.ForegroundColor = current.Foreground;

            if (color.Background.HasValue)
            {
                Console.BackgroundColor = current.Background!.Value;
            }
        });
    }
}
