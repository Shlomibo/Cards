using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;

namespace LameShithead.States.Game;

public record GamePlayState(
    Context Context,
    IConnection<ShitheadGameState, ShitheadMove> Connection,
    bool IsMaster) : State(Context)
{
    protected override Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
