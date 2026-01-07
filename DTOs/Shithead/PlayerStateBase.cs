using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

/// <summary>
/// The base state of a player in the Shithead game.
/// </summary>
public abstract record PlayerStateBase
{
    /// <summary>
    /// The ID of the player.
    /// </summary>
    public int PlayerId { get; init; }

    /// <summary>
    /// Indicates whether the player has won.
    /// </summary>ary>
    public bool Won { get; init; }

    /// <summary>
    /// Indicates if the player accepted they revealed-cards and are ready to start the game.
    /// </summary>
    public bool RevealedCardsAccepted { get; init; }

    /// <summary>
    /// The player's revealed cards.
    /// </summary>ary>
    public required Dictionary<int, Card> RevealedCards { get; init; }

    /// <summary>
    /// The player's undercards, which may be null if not yet revealed.
    /// </summary>
    public required Dictionary<int, Card?> Undercards { get; init; }

}
