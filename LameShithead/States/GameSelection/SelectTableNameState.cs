using System;

namespace LameShithead.States.GameSelection;

public sealed record SelectTableNameState(Context Context, string PlayerName) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        string tableName = await GetValueFromUser("Please enter a table name:", NonEmpty("Table name"), cancellation);

        return new CreateGameConnectionState(Context, PlayerName, tableName);
    }
}
