using System;

namespace LameShithead.States.GameSelection;

public sealed record SelectPlayerNameState(Context Context) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        string playerName = await GetValueFromUser(
            "Please enter your name:",
            NonEmpty("Player name"),
            cancellation);

        return new SelectTableNameState(Context, playerName);
    }
}
