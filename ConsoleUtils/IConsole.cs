namespace ConsoleUtils;

public interface IConsole
{
    Task<string?> ReadLine(CancellationToken cancellation);
    Task WriteLine(CancellationToken cancellation);
    Task WriteLine<T>(T? content, CancellationToken cancellation);
    Task WriteLine<T>(T? content, Color color, CancellationToken cancellation);
    Task Clear(CancellationToken cancellation);
}
