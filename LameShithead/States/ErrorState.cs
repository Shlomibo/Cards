using System;

namespace LameShithead.States;

public sealed record ErrorState(Context Context, Exception Exception) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        await Context.Console.WriteLine($"💩: {Exception}", ConsoleColor.Red, cancellation);
        _ = await Context.Console.ReadLine(cancellation);

        return this;
    }
}
