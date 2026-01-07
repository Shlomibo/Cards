using System;

namespace DTOs.Shithead;

/// <summary>
/// The state of other players in the Shithead game.
/// </summary>
public sealed record OtherPlayersState : PlayerStateBase
{
    /// <summary>
    /// The count of cards in the player's hand.
    /// </summary>
    public int CardsCount { get; init; }
}
