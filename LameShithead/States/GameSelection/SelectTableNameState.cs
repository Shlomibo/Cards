using System;

namespace LameShithead.States.GameSelection;

public sealed record SelectTableNameState : State
{
    protected override Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
