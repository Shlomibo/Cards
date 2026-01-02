using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;

namespace LameShithead.States.GameSelection;

public sealed record WaitForAGameToStart(
    Context Context,
    Connection<ShitheadGameState, ShitheadMove> Connection) :
    GameConnectionStateBase(Context, Connection)
{
    protected override Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
