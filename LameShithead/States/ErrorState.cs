using System;

namespace LameShithead.States;

public sealed record ErrorState(Exception Exception) : State
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation) =>
        this;
}
