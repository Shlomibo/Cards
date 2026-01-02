using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;

namespace LameShithead.States.GameSelection;

public abstract record GameConnectionStateBase(
    Context Context,
    Connection<ShitheadGameState, ShitheadMove> Connection)
    : State(Context)
{
    protected override Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
