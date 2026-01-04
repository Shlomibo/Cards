using ConsoleUtils.Output;

namespace ConsoleUtils;

public interface IConsole
{
    Task<string?> ReadLine(CancellationToken cancellation);
    Task WriteLine(CancellationToken cancellation);
    Task WriteLine(string line, CancellationToken cancellation);
    Task WriteLine(IConsoleOutput output, CancellationToken cancellation);
    Task WriteLine(InterpolatedConsoleOutput output, CancellationToken cancellation);
    Task Clear(CancellationToken cancellation);
}
