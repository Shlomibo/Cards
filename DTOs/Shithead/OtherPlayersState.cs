using System;

namespace DTOs.Shithead;

public sealed record OtherPlayersState : PlayerStateBase
{
    public int CardsCount { get; init; }
}
