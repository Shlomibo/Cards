using System;
using ConsoleUtils.Output;

namespace LameShithead.States;

public sealed record ErrorState(Context Context, Exception Exception) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        await Context.Console.WriteLine(
            ConsoleOutput.Interpolate($"💩: {Exception}")
                .Stylize(Style.Red.Forward),
            cancellation);
        _ = await Context.Console.ReadLine(cancellation);

        return this;
    }
}
