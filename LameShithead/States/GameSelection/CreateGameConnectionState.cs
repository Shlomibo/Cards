using System;
using LameShithead.States.Game;

namespace LameShithead.States.GameSelection;

public sealed record CreateGameConnectionState(Context Context, string PlayerName, string TableName) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        bool isCreatingTable = await GetOptionFromUser(
            "Please select how to proceed:",
            [
                (1, "Create a new table", true),
                (2, "Join an existing table", false)],
                cancellation);

        var connection = isCreatingTable
            ? await Context.ShitheadClient.CreateTable(TableName, PlayerName, cancellation)
            : await Context.ShitheadClient.JoinTable(TableName, PlayerName, cancellation);

        return new GamePlayState(Context, connection, isCreatingTable);
    }
}
