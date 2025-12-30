using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

public sealed record PlayerState : PlayerStateBase
{
    public required Card[] Hand { get; init; }
}
