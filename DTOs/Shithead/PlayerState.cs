using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

/// <summary>
/// The state of the current player in the Shithead game.
/// </summary>
public sealed record PlayerState : PlayerStateBase
{
    /// <summary>
    /// The player's hand of cards.
    /// </summary>
    public required Card[] Hand { get; init; }
}
